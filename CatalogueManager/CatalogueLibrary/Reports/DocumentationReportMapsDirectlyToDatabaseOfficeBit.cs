﻿//specially for MSword file
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using CatalogueLibrary.Data;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DataAccess;
using ReusableLibraryCode.Icons.IconProvision;
using Xceed.Words.NET;

namespace CatalogueLibrary.Reports
{
    /// <summary>
    /// Turns the IMapsDirectlyToDatabaseTable class descriptions generated by DocumentationReportMapsDirectlyToDatabase into a user readable Microsoft Word Docx file 
    /// containing comments, object type name and corresponding icon within RDMP.  This allows the user to see what a Project is and the icon what an ExtractionConfiguration
    /// is etc and for those descriptions/icons to always match 100% the live/installed version of RDMP.
    /// </summary>
    public class DocumentationReportMapsDirectlyToDatabaseOfficeBit : RequiresMicrosoftOffice
    {
        private DocumentationReportMapsDirectlyToDatabase _report;

        public void GenerateReport(ICheckNotifier notifier, IIconProvider iconProvider,params Assembly[] assemblies)
        {
            try
            {
                _report = new DocumentationReportMapsDirectlyToDatabase(assemblies);
                _report.Check(notifier);

                var f = GetUniqueFilenameInWorkArea("RDMPDocumentation");
                using (DocX document = DocX.Create(f.FullName))
                {
                    var t = InsertTable(document,(_report.Summaries.Count *2) +1, 1);
                    
                    //Listing Cell header
                    SetTableCell(t, 0, 0, "Tables");

                    Type[] keys = _report.Summaries.Keys.ToArray();

                    for (int i = 0; i < _report.Summaries.Count; i++)
                    {
                        SetTableCell(t, (i*2) + 1, 0, keys[i].Name);
                        
                        var bmp = iconProvider.GetImage(keys[i]);

                        if (bmp != null)
                            t.Rows[(i*2) + 1].Cells[0].Paragraphs.First().InsertPicture(GetPicture(document, bmp));

                        SetTableCell(t,(i*2) + 2, 0, _report.Summaries[keys[i]]);
                    }

                    document.Save();
                    ShowFile(f);
                }
            }
            catch (Exception e)
            {
                notifier.OnCheckPerformed(new CheckEventArgs("Report generation failed", CheckResult.Fail, e));
            }
        }


        private Bitmap GetImage(Type type, Dictionary<string, Bitmap> imagesDictionary)
        {
            string key = type.Name;

            if (typeof (IFilter).IsAssignableFrom(type))
                key = "Filter";

            if (typeof (IContainer).IsAssignableFrom(type))
                key = "FilterContainer";

            if (typeof(ISqlParameter).IsAssignableFrom(type))
                key = "ParametersNode";
            
            //if it has an image associated with it add it
            if (imagesDictionary.ContainsKey(key))
                return imagesDictionary[key];

            return null;
        }
    }
}
