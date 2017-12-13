using System;
using System.Linq;
using System.Reflection;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.DataLoad;
using CatalogueLibrary.Repositories;
using MapsDirectlyToDatabaseTable;

namespace CatalogueLibrary.ExternalDatabaseServerPatching
{
    public class ANOStoreDatabasePatcher:IPatcher
    {
        public Assembly GetHostAssembly()
        {
            return Assembly.Load("ANOStore");
        }

        public Assembly GetDbAssembly()
        {
            return Assembly.Load("ANOStore.Database");
        }
    }
}