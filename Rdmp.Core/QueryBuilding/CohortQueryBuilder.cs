// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using FAnsi;
using FAnsi.Discovery;
using MapsDirectlyToDatabaseTable;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.Curation.Data.Cohort;
using Rdmp.Core.Providers;
using Rdmp.Core.QueryBuilding.Parameters;
using Rdmp.Core.Repositories;
using ReusableLibraryCode.DataAccess;

namespace Rdmp.Core.QueryBuilding
{
    /// <summary>
    /// Builds complex cohort identification queries by combining subqueries with SQL set operations (UNION / INTERSECT / EXCEPT).  Cohort identification
    /// sub queries fundamentally take the form of 'Select distinct patientId from TableX'.  All the complexity comes in the form of IFilters (WHERE Sql), 
    /// parameters, using cached query results, patient index tables etc.
    /// 
    /// <para>User cohort identification queries are all create under a CohortIdentificationConfiguration which will have a single root CohortAggregateContainer.  A
    /// final count for the number of patients in the cohort can be determined by running the root CohortAggregateContainer.  The user will often want to run each
    /// sub query independently however to get counts for each dataset involved.  Sub queries are defined in AggregateConfigurations.</para>
    /// 
    /// <para>In order to build complex multi table queries across multiple datasets with complex where/parameter/join logic with decent performance RDMP supports 
    /// caching.  Caching involves executing each sub query (AggregateConfiguration) and storing the resulting patient identifier list in an indexed table on
    /// the caching server (See CachedAggregateConfigurationResultsManager).  These cached queries are versioned by the SQL used to generate them (to avoid stale
    /// result lists).  Where available CohortQueryBuilder will use the cached result list instead of running the full query since it runs drastically faster.</para>
    /// 
    /// <para>The SQL code for individual queries is created by CohortQueryBuilderHelper (using AggregateBuilder).</para>
    /// </summary>
    public class CohortQueryBuilder
    {
        private ICoreChildProvider _childProvider;
        private readonly ISqlParameter[] _globals;
        object oSQLLock = new object();
        private string _sql;

        public string SQL
        {
            get
            {
                lock (oSQLLock)
                {
                    if (SQLOutOfDate)
                        RegenerateSQL();
                    return _sql;
                }
            }
        }
        
        public int TopX { get; set; }

        private CohortAggregateContainer container;
        private AggregateConfiguration configuration;
        
        /// <summary>
        /// The server on which the query must run.  This is used to accumulate all sub-queries and ensures
        /// that they don't stray into other servers (but they can be into different databases within the same server / server type). 
        /// </summary>
        public DiscoveredServer TargetServer => results.TargetServer;

        public ExternalDatabaseServer CacheServer
        {
            get { return _cacheServer; }
            set
            {
                _cacheServer = value;
                SQLOutOfDate = true;
            }
        }

        public ParameterManager ParameterManager = new ParameterManager();

        public int CountOfSubQueries { get { return results != null? results.Dependencies.Count:- 1; } }
        public int CountOfCachedSubQueries { get { return results != null ? results.Dependencies.Count(d=>d.SqlFullyCached != null) : -1; } }

        private CohortQueryBuilderHelper helper;
        private CohortQueryBuilderResult results;

        #region constructors
        //Constructors - This one is the base one called by all others
        private CohortQueryBuilder(IEnumerable<ISqlParameter> globals, ICoreChildProvider childProvider)
        {
            _childProvider = childProvider;
            globals = globals ?? new ISqlParameter[] {};
            _globals = globals.ToArray();
            TopX = -1;
            
            SQLOutOfDate = true;

            foreach (ISqlParameter parameter in _globals)
                ParameterManager.AddGlobalParameter(parameter);
        }

        public CohortQueryBuilder(CohortIdentificationConfiguration configuration, ICoreChildProvider childProvider):this(configuration.GetAllParameters(),childProvider)
        {
            if (configuration == null)
                throw new QueryBuildingException("Configuration has not been set yet");

            if (configuration.RootCohortAggregateContainer_ID == null)
                throw new QueryBuildingException("Root container not set on CohortIdentificationConfiguration " + configuration);

            if (configuration.QueryCachingServer_ID != null)
                CacheServer = configuration.QueryCachingServer;

            //set ourselves up to run with the root container
            container = configuration.RootCohortAggregateContainer;

            SetChildProviderIfNull();
            
        }

        public CohortQueryBuilder(CohortAggregateContainer c,IEnumerable<ISqlParameter> globals, ICoreChildProvider childProvider): this(globals,childProvider)
        {
            //set ourselves up to run with the root container
            container = c;

            SetChildProviderIfNull();
        }
        public CohortQueryBuilder(AggregateConfiguration config, IEnumerable<ISqlParameter> globals, ICoreChildProvider childProvider): this(globals,childProvider)
        {
            //set ourselves up to run with the root container
            configuration = config;

            SetChildProviderIfNull();
        }
        private void SetChildProviderIfNull()
        {
            if (_childProvider == null)
                _childProvider = new CatalogueChildProvider(
                    configuration?.CatalogueRepository ?? container.CatalogueRepository, null, null);
        }


        #endregion

        public string GetDatasetSampleSQL(int topX = 1000,ICoreChildProvider childProvider = null)
        {
            if(configuration == null)
                throw new NotSupportedException("Can only generate select * statements when constructed for a single AggregateConfiguration, this was constructed with a container as the root entity (it may even reflect a UNION style query that spans datasets)");

            RecreateHelpers(new QueryBuilderCustomArgs("*", "" /*removes distinct*/, topX));

            results.BuildFor(configuration);
            
            
            string sampleSQL = results.Sql;

            string parameterSql = "";

            //get resolved parameters for the select * query
            var finalParams = ParameterManager.GetFinalResolvedParametersList().ToArray();

            if(finalParams.Any())
            {
                foreach (ISqlParameter param in finalParams)
                    parameterSql += QueryBuilder.GetParameterDeclarationSQL(param);

                return parameterSql + Environment.NewLine + sampleSQL;
            }
            
            return sampleSQL;
        }
        
        public void RegenerateSQL()
        {
            RecreateHelpers(null);

            results.StopContainerWhenYouReach = _stopContainerWhenYouReach;

            if (container != null)
                results.BuildFor(container);    //user constructed us with a container (and possibly subcontainers even - any one of them chock full of aggregates)
            else
                results.BuildFor(configuration);//user constructed us without a container, he only cares about 1 aggregate

            _sql = results.Sql;
  
            //Still finalise the ParameterManager even if we are not writting out the parameters so that it is in the Finalized state
            var finalParameters = ParameterManager.GetFinalResolvedParametersList();

            if(!DoNotWriteOutParameters)
            {
                string parameterSql = "";

                //add the globals
                foreach (ISqlParameter param in finalParameters)
                    parameterSql += QueryBuilder.GetParameterDeclarationSQL(param);

                _sql =  parameterSql + _sql;
            }
            SQLOutOfDate = false;
        }

        private void RecreateHelpers(QueryBuilderCustomArgs customizations)
        {
            helper = new CohortQueryBuilderHelper(ParameterManager);
            results = new CohortQueryBuilderResult(CacheServer,_childProvider, helper,customizations);
        }

        /// <summary>
        /// Tells the Builder not to write out parameter SQL, unlike AggregateBuilder this will not clear the ParameterManager it will just hide them from the SQL output
        /// </summary>
        public bool DoNotWriteOutParameters
        {
            get { return _doNotWriteOutParameters; }
            set
            {
                _doNotWriteOutParameters = value;
                SQLOutOfDate = true;
            }
        }
        
        public bool SQLOutOfDate { get; set; }

        private IOrderable _stopContainerWhenYouReach;
        private bool _doNotWriteOutParameters;
        private ExternalDatabaseServer _cacheServer;
        

        public IOrderable StopContainerWhenYouReach
        {
            get { return _stopContainerWhenYouReach; }
            set
            {
                _stopContainerWhenYouReach = value;
                SQLOutOfDate = true;
            }
        }
    }
}
