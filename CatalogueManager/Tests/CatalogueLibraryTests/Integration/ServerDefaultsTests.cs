// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using NUnit.Framework;
using Rdmp.Core.CatalogueLibrary.Data;
using Rdmp.Core.CatalogueLibrary.Data.Defaults;
using Tests.Common;

namespace CatalogueLibraryTests.Integration
{
    public class ServerDefaultsTests : DatabaseTests
    {
        [Test]
        public void CreateNewExternalServerAndConfigureItAsDefault()
        {

            ServerDefaults defaults = new ServerDefaults(CatalogueRepository);

            var databaseServer = new ExternalDatabaseServer(CatalogueRepository, "Deleteme",null);

            try
            {
                Assert.AreEqual("Deleteme",databaseServer.Name);
                databaseServer.Password = "nothing"; //automatically encrypts password

                Assert.AreNotEqual("nothing",databaseServer.Password);//should not match what we just set it to
                Assert.AreEqual("nothing", databaseServer.GetDecryptedPassword());//should match what we set it to because of explicit call to decrypt

                databaseServer.Server = "Bob";
                databaseServer.Database = "TEST";
                databaseServer.SaveToDatabase();
                
                Catalogue cata = new Catalogue(CatalogueRepository, "TestCatalogueFor_CreateNewExternalServerAndConfigureItAsDefault");
                cata.DeleteInDatabase();

            }
            finally
            {
                databaseServer.DeleteInDatabase();
            }

        }
    }
}
