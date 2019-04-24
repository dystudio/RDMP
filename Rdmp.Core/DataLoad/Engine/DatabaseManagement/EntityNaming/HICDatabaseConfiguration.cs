// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Text.RegularExpressions;
using FAnsi.Discovery;
using Rdmp.Core.CatalogueLibrary.Data;
using Rdmp.Core.CatalogueLibrary.Data.DataLoad;
using Rdmp.Core.CatalogueLibrary.Data.Defaults;
using Rdmp.Core.CatalogueLibrary.Data.EntityNaming;
using ReusableLibraryCode.DataAccess;

namespace Rdmp.Core.DataLoad.Engine.DatabaseManagement.EntityNaming
{
    /// <summary>
    /// Wrapper for StandardDatabaseHelper (which tells you where RAW, STAGING and LIVE databases are during data load execution).  This class exists for two reasons
    /// 
    /// <para>Firstly to decide (based on IAttachers) whether RAW tables need to be scripted or whether they will appear magically during DLE execution (e.g. by attaching 
    /// an MDF file).</para>
    /// 
    /// <para>Secondly to allow for overriding the RAW database server (which defaults to localhost).  It is a good idea to have RAW on a different server to LIVE/STAGING
    /// in order to reduce the risk incorrectly referencing tables in LIVE in Adjust RAW scripts etc.</para>
    /// </summary>
    public class HICDatabaseConfiguration
    {
        public StandardDatabaseHelper DeployInfo { get; set; }
        public bool RequiresStagingTableCreation { get; set; }

        public INameDatabasesAndTablesDuringLoads DatabaseNamer
        {
            get { return DeployInfo.DatabaseNamer; }
        }

        /// <summary>
        /// Optional Regex for fields which will be ignored at migration time between STAGING and LIVE (e.g. hic_ columns).  This prevents incidental fields like
        /// valid from, data load run id etc from resulting in live table UPDATEs.
        /// 
        /// <para>hic_ columns will always be ignored regardless of this setting</para>
        /// </summary>
        public Regex UpdateButDoNotDiff { get; set; }

        /// <summary>
        /// Preferred Constructor, creates RAW, STAGING, LIVE connection strings based on the data access points in the LoadMetadata, also respects the ServerDefaults for RAW override (if any)
        /// </summary>
        /// <param name="lmd"></param>
        /// <param name="namer"></param>
        public HICDatabaseConfiguration(ILoadMetadata lmd, INameDatabasesAndTablesDuringLoads namer = null):
            this(lmd.GetDistinctLiveDatabaseServer(), namer, lmd.CatalogueRepository.GetServerDefaults(),lmd.OverrideRAWServer)
        {
        }

        /// <summary>
        /// Constructor for use in tests, if possible use the LoadMetadata constructor instead
        /// </summary>
        /// <param name="liveServer">The live server where the data is held, IMPORTANT: this must contain InitialCatalog parameter</param>
        /// <param name="namer">optionally lets you specify how to pick database names for the temporary bubbles STAGING and RAW</param>
        /// <param name="defaults">optionally specifies the location to get RAW default server from</param>
        /// <param name="overrideRAWServer">optionally specifies an explicit server to use for RAW</param>
        public HICDatabaseConfiguration(DiscoveredServer liveServer, INameDatabasesAndTablesDuringLoads namer = null, IServerDefaults defaults = null, IExternalDatabaseServer overrideRAWServer = null)
        {
            //respects the override of LIVE server
            string liveDatabase = liveServer.Helper.GetCurrentDatabase(liveServer.Builder);

            if (string.IsNullOrWhiteSpace(liveDatabase))
                throw new Exception("Cannot load live without having a unique live named database");

            // Default namer
            if (namer == null)
                namer = new FixedStagingDatabaseNamer(liveDatabase);

            DiscoveredServer rawServer = null;

            //if there are defaults
            if(overrideRAWServer == null && defaults != null)
                overrideRAWServer = defaults.GetDefaultFor(PermissableDefaults.RAWDataLoadServer);//get the raw default if there is one
            
            //if there was defaults and a raw default server
            if (overrideRAWServer != null)
                rawServer = DataAccessPortal.GetInstance().ExpectServer(overrideRAWServer, DataAccessContext.DataLoad, false); //get the raw server connection
            else
                rawServer = liveServer; //there is no raw override so we will have to use the live database for RAW too.

            //populates the servers -- note that an empty rawServer value passed to this method makes it the localhost
            DeployInfo = new StandardDatabaseHelper(liveServer.GetCurrentDatabase(), namer,rawServer);
            
            RequiresStagingTableCreation = true;
        }
    }
}
