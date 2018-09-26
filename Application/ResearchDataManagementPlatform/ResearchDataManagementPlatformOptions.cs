﻿using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace ResearchDataManagementPlatform
{
    /// <summary>
    /// Defines the command line arguments of ResearchDataManagementPlatform.exe when run from the command line / shortcut
    /// </summary>
    public class ResearchDataManagementPlatformOptions
    {
        [Option('c', Required = false, HelpText = @"Connection string to the main Catalogue RDMP database")]
        public string CatalogueConnectionString { get; set; }

        [Option('d', Required = false, HelpText = @"Connection string to the main DataExport RDMP database")]
        public string DataExportConnectionString { get; set; }


        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Load Default User Connection String Settings", new ResearchDataManagementPlatformOptions());
                yield return new Example("Use These Connection Strings", new ResearchDataManagementPlatformOptions { CatalogueConnectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=RDMP_Catalogue;Integrated Security=True", DataExportConnectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=RDMP_DataExport;Integrated Security=True" });
            }
        }
    }
}