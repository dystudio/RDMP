﻿// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.DataQualityEngine;
using Rdmp.Core.Repositories;

namespace Rdmp.Core.Reports
{
    /// <summary>
    /// Create a custom report e.g. markdown, xml etc by taking a template file and replicating it with replacements for each <see cref="Catalogue"/> property
    /// </summary>
    public class CustomMetadataReport
    {
        /// <summary>
        /// Substitutions that are used during template value replacement e.g. $Name => Catalogue.Name
        /// </summary>

        Dictionary<string,Func<Catalogue,object>> Replacements = new Dictionary<string, Func<Catalogue,object>>();

        /// <summary>
        /// Substitutions that are used during template value replacement when inside a '$foreach CatalogueItem' block e.g. $Name => CatalogueItem.Name
        /// </summary>

        Dictionary<string,Func<CatalogueItem,object>> ReplacementsCatalogueItem = new Dictionary<string, Func<CatalogueItem,object>>();

        /// <summary>
        /// Control line that begins looping CatalogueItems of a Catalogue
        /// </summary>
        public const string LoopCatalogueItems = "$foreach CatalogueItem";

        /// <summary>
        /// Ends looping
        /// </summary>
        public const string EndLoop = "$end";

        private readonly IDetermineDatasetTimespan _timespanCalculator = new DatasetTimespanCalculator();

        /// <summary>
        /// Specify a replacement for newlines when found in fields e.g. with space.  Leave as null to leave newlines intact.
        /// </summary>
        public string NewlineSubstitution { get; internal set; }

        public CustomMetadataReport()
        {
            //add basic properties
            foreach (var prop in typeof(Catalogue).GetProperties())
                Replacements.Add("$" + prop.Name, (s) => prop.GetValue(s));
            
            //add basic properties CatalogueItem
            foreach (var prop in typeof(CatalogueItem).GetProperties())
                ReplacementsCatalogueItem.Add("$" + prop.Name, (s) => prop.GetValue(s));

            Replacements.Add("$StartDate",
                (c) => _timespanCalculator?.GetMachineReadableTimepsanIfKnownOf(c, true, out _)?.Item1?.ToString());
            Replacements.Add("$EndDate",
                (c) => _timespanCalculator?.GetMachineReadableTimepsanIfKnownOf(c, true, out _)?.Item2?.ToString());
            Replacements.Add("$DateRange",
                (c) => _timespanCalculator?.GetHumanReadableTimepsanIfKnownOf(c, true, out _));
        }
        
        /// <summary>
        /// Reads the contents of <paramref name="template"/> and generates one or more files (see <paramref name="oneFile"/>) by substituting tokens (e.g. $Name) for the values in the provided <paramref name="catalogues"/>
        /// </summary>
        /// <param name="catalogues">All catalogues that you want to produce metadata for</param>
        /// <param name="outputDirectory">The directory to write output file(s) into</param>
        /// <param name="template">Template file with free text and substitutions (e.g. $Name).  Also supports looping e.g. $foreach CatalogueItem</param>
        /// <param name="fileNaming">Determines how output file(s) will be named in the <paramref name="outputDirectory"/>.  Supports substitution e.g. $Name.md</param>
        /// <param name="oneFile">True to concatenate the results together and output in a single file.  If true then <paramref name="fileNaming"/> should not contain substitutions.  If false then <paramref name="fileNaming"/> should contain substitutions (e.g. $Name.doc) to prevent duplicate file names</param>
        public void GenerateReport(Catalogue[] catalogues, DirectoryInfo outputDirectory, FileInfo template, string fileNaming, bool oneFile)
        {
            if(catalogues == null || !catalogues.Any())
                return;
            
            var templateBody = File.ReadAllLines(template.FullName);

            string outname = DoReplacements(new []{fileNaming},catalogues.First()).Trim();

            StreamWriter outFile = null;
            
            if(oneFile)
                outFile = new StreamWriter(File.Create(Path.Combine(outputDirectory.FullName, outname)));

            foreach (Catalogue catalogue in catalogues)
            {
                var newContents = DoReplacements(templateBody, catalogue);

                if (oneFile) 
                    outFile.WriteLine(newContents);
                else
                {
                    string filename = DoReplacements(new[] {fileNaming}, catalogue).Trim();

                    using (var sw = new StreamWriter(Path.Combine(outputDirectory.FullName,filename)))
                    {
                        sw.Write(newContents);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            outFile?.Flush();
            outFile?.Dispose();
        }

        private string DoReplacements(string[] strs, Catalogue catalogue)
        {
            StringBuilder sb = new StringBuilder();

            for (var index = 0; index < strs.Length; index++)
            {
                var str = strs[index];
                string copy = str;

                if (str.Trim().Equals(LoopCatalogueItems, StringComparison.CurrentCultureIgnoreCase))
                {
                    index = DoReplacements(strs, index, out copy,catalogue.CatalogueItems) + 1;
                }
                else
                {
                    foreach (var r in Replacements)
                        if (copy.Contains(r.Key))
                            copy = copy.Replace(r.Key, ValueToString(r.Value(catalogue)));
                }

                sb.AppendLine(copy.Trim());
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Consumes from $foreach to $end looping <paramref name="catalogueItems"/> to produce output rows of string data
        /// </summary>
        /// <param name="strs">The original input in its entirety</param>
        /// <param name="index">The line in <paramref name="strs"/> in which the foreach was detected</param>
        /// <param name="result">The results of consuming the foreach block</param>
        /// <param name="catalogueItems"></param>
        /// <returns>The index in <paramref name="strs"/> where the $end was detected</returns>
        private int DoReplacements(string[] strs, int index, out string result, CatalogueItem[] catalogueItems)
        {
            // The foreach template block as extracted from strs
            StringBuilder block = new StringBuilder();

            //the result of printing out the block once for each CatalogueItem item (with replacements)
            StringBuilder sbResult = new StringBuilder();

            int i = index+1;
            bool blockTerminated = false;

            //starting on the next line after $foreach until the end of the file
            for (; i < strs.Length; i++)
            {
                var current = strs[i];

                if (current.Trim().Equals(EndLoop, StringComparison.CurrentCultureIgnoreCase))
                {
                    blockTerminated = true;
                    break;
                }

                if(current == LoopCatalogueItems)
                    throw new CustomMetadataReportException($"Error, encountered '{current}' on line {i+1} before the end of current block which started on line {index +1}.  Make sure to add {EndLoop} at the end of each loop",i+1);

                block.AppendLine(current);
            }

            if(!blockTerminated)
                throw new CustomMetadataReportException($"Expected {EndLoop} to match $foreach which started on line {index+1}",index+1);

            foreach (CatalogueItem ci in catalogueItems) 
                sbResult.AppendLine(DoReplacements(block.ToString(), ci));

            result = sbResult.ToString();

            return i;
        }

        /// <summary>
        /// Returns a string representation suitable for adding to a template output based on the input object (which may be null)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string ValueToString(object v)
        {
            return ReplaceNewlines(v?.ToString() ?? "");
        }

        public string ReplaceNewlines(string input)
        {
            if(input != null && NewlineSubstitution != null)
                return Regex.Replace(input,"[\r]?\n",NewlineSubstitution);

            return input;
        }

        /// <summary>
        /// Replace all templated strings (e.g. $Name) in the <paramref name="template"/> with the corresponding values in the <paramref name="catalogueItem"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="catalogueItem"></param>
        /// <returns></returns>
        private string DoReplacements(string template, CatalogueItem catalogueItem)
        {
            foreach (var r in ReplacementsCatalogueItem)
                if (template.Contains(r.Key))
                    template = template.Replace(r.Key, ValueToString(r.Value(catalogueItem)));

            return template.Trim();
        }
    }
}
