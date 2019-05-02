// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Data.SqlClient;
using FAnsi.Discovery;
using MapsDirectlyToDatabaseTable.Versioning;
using Rdmp.Core.Databases;
using Rdmp.Core.Repositories;
using ReusableLibraryCode.Checks;

namespace Rdmp.Core.CommandLine.DatabaseCreation
{
    /// <summary>
    /// Creates RDMP core databases (logging, DQE, Catalogue, DataExport) in the given database server.  Also creates initial
    /// pipelines for common activities.
    /// </summary>
    public class PlatformDatabaseCreation
    {
        public const string DefaultCatalogueDatabaseName = "Catalogue";
        public const string DefaultDataExportDatabaseName = "DataExport";
        public const string DefaultDQEDatabaseName = "DQE";
        public const string DefaultLoggingDatabaseName = "Logging";

        public void CreatePlatformDatabases(PlatformDatabaseCreationOptions options)
        {
            
            Create(DefaultCatalogueDatabaseName, new CataloguePatcher(), options);
            Create(DefaultDataExportDatabaseName, new DataExportPatcher(), options);

            var dqe = Create(DefaultDQEDatabaseName, new DataQualityEnginePatcher(), options);
            var logging = Create(DefaultLoggingDatabaseName, new LoggingDatabasePatcher(), options);

            CatalogueRepository.SuppressHelpLoading = true;

            if (!options.SkipPipelines)
            {
                var creator = new CataloguePipelinesAndReferencesCreation(new PlatformDatabaseCreationRepositoryFinder(options), logging, dqe);
                creator.Create();
            }
        }

        private SqlConnectionStringBuilder Create(string databaseName, IPatcher patcher, PlatformDatabaseCreationOptions options)
        {
            SqlConnection.ClearAllPools();

            var builder = options.GetBuilder(databaseName);

            DiscoveredDatabase db = new DiscoveredServer(builder).ExpectDatabase(builder.InitialCatalog);

            if (options.DropDatabases && db.Exists())
            {
                Console.WriteLine("Dropping Database:" + builder.InitialCatalog);
                db.Drop();
            }
            
            MasterDatabaseScriptExecutor executor = new MasterDatabaseScriptExecutor(builder.ConnectionString);
            executor.BinaryCollation = options.BinaryCollation;
            executor.CreateAndPatchDatabase(patcher,new AcceptAllCheckNotifier());
            Console.WriteLine("Created " + builder.InitialCatalog + " on server " + builder.DataSource);
            
            return builder;
        }
    }
}