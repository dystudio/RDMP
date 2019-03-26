// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using CatalogueLibrary.Repositories;
using MapsDirectlyToDatabaseTable;
using RDMPStartup;

namespace DatabaseCreation
{
    /// <summary>
    /// IRDMPPlatformRepositoryServiceLocator which identifies the location of Catalogue and Data Export databases during the runtime of DatabaseCreation.exe
    /// 
    /// <para>Since these connection strings are part of the command line arguments to DatabaseCreation.exe it's a pretty simple class!</para>
    /// </summary>
    public class DatabaseCreationRepositoryFinder : IRDMPPlatformRepositoryServiceLocator
    {
        private readonly LinkedRepositoryProvider _linkedRepositoryProvider;

        public CatalogueRepository CatalogueRepository
        {
            get { return _linkedRepositoryProvider.CatalogueRepository; }
        }

        public IDataExportRepository DataExportRepository
        {
            get { return _linkedRepositoryProvider.DataExportRepository; }
        }

        public IMapsDirectlyToDatabaseTable GetArbitraryDatabaseObject(string repositoryTypeName, string databaseObjectTypeName, int objectID)
        {
            return _linkedRepositoryProvider.GetArbitraryDatabaseObject(repositoryTypeName, databaseObjectTypeName, objectID);
        }

        public bool ArbitraryDatabaseObjectExists(string repositoryTypeName, string databaseObjectTypeName, int objectID)
        {
            return _linkedRepositoryProvider.ArbitraryDatabaseObjectExists(repositoryTypeName,databaseObjectTypeName,objectID);
        }

        public DatabaseCreationRepositoryFinder(DatabaseCreationProgramOptions options)
        {
            var cata = options.GetBuilder(DatabaseCreationProgram.DefaultCatalogueDatabaseName);
            var export = options.GetBuilder(DatabaseCreationProgram.DefaultDataExportDatabaseName);
            
            _linkedRepositoryProvider = new LinkedRepositoryProvider(cata.ConnectionString, export.ConnectionString);

        }
    }
}
