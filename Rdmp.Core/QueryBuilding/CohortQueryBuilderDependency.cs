﻿// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.Curation.Data.Cohort;
using Rdmp.Core.Curation.Data.Cohort.Joinables;
using Rdmp.Core.Providers;
using Rdmp.Core.QueryBuilding.Parameters;
using Rdmp.Core.QueryCaching.Aggregation;

namespace Rdmp.Core.QueryBuilding
{
    /// <summary>
    /// A single cohort set in a <see cref="CohortIdentificationConfiguration"/> which selects specific patients from the database by their unique <see cref="IColumn.IsExtractionIdentifier"/>.
    /// Can include a join to a patient index table.  This class stores the cached (if available) uncached and partially Cached SQL for relevant subsections of the query (and the whole query).
    /// So that the decision about whether to use the cache can be delayed till later
    /// 
    /// </summary>
    public class CohortQueryBuilderDependency
    {
        private readonly ICoreChildProvider _childProvider;
        
        /// <summary>
        /// The primary table being queried
        /// </summary>
        public AggregateConfiguration CohortSet { get; }

        /// <summary>
        /// The relationship object describing the JOIN relationship between <see cref="CohortSet"/> and another optional table
        /// </summary>
        public JoinableCohortAggregateConfigurationUse PatientIndexTableIfAny { get; }
        
        /// <summary>
        /// The column in the <see cref="CohortSet"/> that is marked <see cref="IColumn.IsExtractionIdentifier"/>
        /// </summary>
        public AggregateDimension ExtractionIdentifierColumn { get;}

        /// <summary>
        /// The aggregate (query) referenced by <see cref="PatientIndexTableIfAny"/>
        /// </summary>
        public AggregateConfiguration JoinedTo { get; }

        /// <summary>
        /// The raw SQL that can be used to join the <see cref="CohortSet"/> and <see cref="PatientIndexTableIfAny"/> (if there is one).  Null if they exist
        /// on different servers (this is allowed only if the <see cref="CohortSet"/> is on the same server as the cache while the <see cref="PatientIndexTableIfAny"/>
        /// is remote).
        ///
        /// <para>This SQL does not include the parameter declaration SQL since it is designed for nesting e.g. in UNION / INTERSECT / EXCEPT hierarchy</para>
        /// </summary>
        public CohortQueryBuilderDependencySql SqlCacheless { get; private set; }
        
        /// <summary>
        /// The raw SQL for the <see cref="CohortSet"/> with a join against the cached artifact for the <see cref="PatientIndexTableIfAny"/>
        /// </summary>
        public CohortQueryBuilderDependencySql SqlPartiallyCached { get;  private set;}
        
        /// <summary>
        /// Sql for a single cache fetch  that pulls the cached result of the <see cref="CohortSet"/> joined to <see cref="PatientIndexTableIfAny"/> (if there was any)
        /// </summary>
        public CohortQueryBuilderDependencySql SqlFullyCached { get;  private set;}

        public CohortQueryBuilderDependencySql SqlJoinableCacheless { get; private set; }
        
        public CohortQueryBuilderDependencySql SqlJoinableCached { get; private set; }

        public CohortQueryBuilderDependency(AggregateConfiguration cohortSet,
            JoinableCohortAggregateConfigurationUse patientIndexTableIfAny, ICoreChildProvider childProvider)
        {
            _childProvider = childProvider;
            CohortSet = cohortSet;
            PatientIndexTableIfAny = patientIndexTableIfAny;

            //record the IsExtractionIdentifier column for the log (helps with debugging count issues)
            var eis = cohortSet?.AggregateDimensions?.Where(d => d.IsExtractionIdentifier).ToArray();
            
            //Multiple IsExtractionIdentifier columns is a big problem but it's handled elsewhere
            if(eis != null && eis.Length == 1)
                ExtractionIdentifierColumn = eis[0];
            
            if (PatientIndexTableIfAny != null)
            {
                var join = _childProvider.AllJoinables.SingleOrDefault(j =>
                    j.ID == PatientIndexTableIfAny.JoinableCohortAggregateConfiguration_ID);

                if(join == null)
                    throw new Exception("ICoreChildProvider did not know about the provided patient index table");

                JoinedTo = _childProvider.AllAggregateConfigurations.SingleOrDefault(ac =>
                    ac.ID == join.AggregateConfiguration_ID);

                if(JoinedTo == null)
                    throw new Exception("ICoreChildProvider did not know about the provided patient index table AggregateConfiguration");
            }
        }

        public override string ToString()
        {
            return CohortSet.Name + (JoinedTo != null ? PatientIndexTableIfAny.JoinType + " JOIN " + JoinedTo.Name : "");
        }

        public void Build(CohortQueryBuilderResult parent,ISqlParameter[] globals)
        {
            bool isSolitaryPatientIndexTable = CohortSet.IsJoinablePatientIndexTable();
            
            //Includes the parameter declaration and no rename operations (i.e. couldn't be used for building the tree but can be used for cache hit testing).
            if (JoinedTo != null)
            {
                SqlJoinableCacheless = parent.Helper.GetSQLForAggregate(JoinedTo,
                    new QueryBuilderArgs(new QueryBuilderCustomArgs(), //don't respect customizations in the inception bit!
                        globals));
                SqlJoinableCached = GetCachFetchSqlIfPossible(parent,JoinedTo,SqlJoinableCacheless,true);
            }
            
            if (isSolitaryPatientIndexTable)
            {
                //explicit execution of a patient index table on it's own
                //the full uncached SQL for the query
                SqlCacheless = parent.Helper.GetSQLForAggregate(CohortSet,new QueryBuilderArgs(parent.Customise,globals));

                if(SqlJoinableCached != null)
                    throw new QueryBuildingException("Patient index tables can't use other patient index tables!");
            }
            else
            {
                //the full uncached SQL for the query
                SqlCacheless = parent.Helper.GetSQLForAggregate(CohortSet,
                    new QueryBuilderArgs(PatientIndexTableIfAny, JoinedTo,
                        SqlJoinableCacheless,parent.Customise,globals));

                
                //if the joined to table is cached we can generate a partial too with full sql for the outer sql block and a cache fetch join
                if (SqlJoinableCached != null)
                    SqlPartiallyCached = parent.Helper.GetSQLForAggregate(CohortSet,
                        new QueryBuilderArgs(PatientIndexTableIfAny, JoinedTo,
                            SqlJoinableCached,parent.Customise,globals));
            }
            
            //We would prefer a cache hit on the exact uncached SQL
            SqlFullyCached = GetCachFetchSqlIfPossible(parent, CohortSet, SqlCacheless, isSolitaryPatientIndexTable);

            //but if that misses we would take a cache hit of an execution of the SqlPartiallyCached
            if(SqlFullyCached == null && SqlPartiallyCached != null)
                SqlFullyCached = GetCachFetchSqlIfPossible(parent,CohortSet,SqlPartiallyCached,isSolitaryPatientIndexTable);
        }

        private CohortQueryBuilderDependencySql GetCachFetchSqlIfPossible(CohortQueryBuilderResult parent,AggregateConfiguration aggregate, CohortQueryBuilderDependencySql sql, bool isPatientIndexTable)
        {
            if (parent.CacheManager == null)
                return null;

            string parameterSql = QueryBuilder.GetParameterDeclarationSQL(sql.ParametersUsed.Clone().GetFinalResolvedParametersList());

            string hitTestSql = parameterSql + sql.Sql;

            var existingTable = parent.CacheManager.GetLatestResultsTable(aggregate, isPatientIndexTable
                ?AggregateOperation.JoinableInceptionQuery:AggregateOperation.IndexedExtractionIdentifierList , hitTestSql);
                
            //if there is a cached entry matching the cacheless SQL then we can just do a select from it (in theory)
            if (existingTable != null)
            {
                string sqlCachFetch = CachedAggregateConfigurationResultsManager.CachingPrefix + aggregate.Name + @"*/" + Environment.NewLine +
                    "select * from " + existingTable.GetFullyQualifiedName() + Environment.NewLine;

                //Cache fetch does not require any parameters
                return new CohortQueryBuilderDependencySql(sqlCachFetch,new ParameterManager());
            }
                    
            
            return null;
        }

        public string DescribeCachedState()
        {
            if (SqlFullyCached != null)
                return "Fully Cached";

            if (SqlPartiallyCached != null)
                return "Partially Cached";

            if (SqlCacheless != null)
                return "Not Cached";

            return "Not Built";
        }
    }
}