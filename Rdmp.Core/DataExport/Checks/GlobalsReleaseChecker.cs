// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using MapsDirectlyToDatabaseTable;
using Rdmp.Core.CatalogueLibrary.Data;
using Rdmp.Core.CatalogueLibrary.Repositories;
using Rdmp.Core.CatalogueLibrary.Repositories.Construction;
using Rdmp.Core.DataExport.Data.DataTables;
using Rdmp.Core.DataExport.DataRelease.Potential;
using Rdmp.Core.DataExport.ExtractionTime;
using Rdmp.Core.DataExport.ExtractionTime.ExtractionPipeline.Destinations;
using ReusableLibraryCode.Checks;

namespace Rdmp.Core.DataExport.Checks
{
    /// <summary>
    /// Checks the release state of the Globals that should have been extracted as part of the given <see cref="ExtractionConfiguration"/>.  If they
    /// are missing then the overall release should not be run.
    /// </summary>
    public class GlobalsReleaseChecker : ICheckable
    {
        private readonly IRDMPPlatformRepositoryServiceLocator _repositoryLocator;
        private readonly IExtractionConfiguration[] _configurations;
        private readonly IMapsDirectlyToDatabaseTable _globalToCheck;

        /// <summary>
        /// Sets up checking of extracted global artifacts of the <paramref name="extractionConfigurations"/> to ensure that none have been missed and all are in 
        /// a fit state to be shipped off in a release package.  You require one <see cref="GlobalsReleaseChecker"/> per global (which should be passeed as
        /// <paramref name="globalToCheck"/>).
        /// </summary>
        /// <param name="repositoryLocator"></param>
        /// <param name="extractionConfigurations"></param>
        /// <param name="globalToCheck">Determines the return of <see cref="GetEvaluator"/>.  Pass a global </param>
        public GlobalsReleaseChecker(IRDMPPlatformRepositoryServiceLocator repositoryLocator, IExtractionConfiguration[] extractionConfigurations, IMapsDirectlyToDatabaseTable globalToCheck = null)
        {
            _repositoryLocator = repositoryLocator;
            _configurations = extractionConfigurations;
            _globalToCheck = globalToCheck;
        }

        /// <summary>
        /// Only call if globalToCheck was provided during construction.  This method looks up the database to find a <see cref="SupplementalExtractionResults"/> record
        /// that audits the extraction of the global.  If no audit exists then a <see cref="NoGlobalReleasePotential"/> is returned (not releasable).  Otherwise a 
        /// <see cref="GlobalReleasePotential"/> is generated by the <see cref="IExecuteDatasetExtractionDestination"/> reported in the audit (e.g. if the extraction
        /// was to database it might check for tables matching a <see cref="SupportingSQLTable"/>).
        /// </summary>
        /// <returns></returns>
        public GlobalReleasePotential GetEvaluator()
        {
            if(_globalToCheck == null)
                throw new NotSupportedException("You cannot call GetEvaluator when you provided no globalToCheck during construction");

            var globalResult = _configurations.SelectMany(c => c.SupplementalExtractionResults)
                                                  .Distinct()
                                                  .FirstOrDefault(ser => ser.IsReferenceTo(_globalToCheck));

            if (globalResult == null)
                return new NoGlobalReleasePotential(_repositoryLocator, null, _globalToCheck);
            
            //it's been extracted!, who extracted it?
            var destinationThatExtractedIt = (IExecuteDatasetExtractionDestination)new ObjectConstructor().Construct(globalResult.GetDestinationType());

            //destination tell us how releasable it is
            return destinationThatExtractedIt.GetGlobalReleasabilityEvaluator(_repositoryLocator, globalResult, _globalToCheck);
        }

        /// <summary>
        /// Checks extraction directory for unexpected files.  Call <see cref="GetEvaluator"/> if want to know whether a specific global artifact is in a valid 
        /// state.
        /// </summary>
        /// <param name="notifier"></param>
        public void Check(ICheckNotifier notifier)
        {
            // checks for pollution in the globals directory
            foreach (var extractionConfiguration in _configurations)
            {
                var allExtracted = extractionConfiguration.SupplementalExtractionResults.Where(ser => IsValidPath(ser.DestinationDescription));
                var extractDir = extractionConfiguration.GetProject().ExtractionDirectory;
                var folder = new ExtractionDirectory(extractDir, extractionConfiguration).GetGlobalsDirectory();

                var unexpectedDirectories = folder.EnumerateDirectories().Where(d => !d.Name.Equals("SupportingDocuments")).ToList();

                if (unexpectedDirectories.Any())
                    notifier.OnCheckPerformed(new CheckEventArgs("Unexpected directories found in extraction directory (" + 
                                                                  String.Join(",", unexpectedDirectories.Select(d => d.FullName)) + 
                                                                  ". Pollution of extract directory is not permitted.", CheckResult.Fail));

                var unexpectedFiles = folder.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(f => allExtracted.All(ae => ae.DestinationDescription != f.FullName)).ToList();

                if (unexpectedFiles.Any())
                    notifier.OnCheckPerformed(new CheckEventArgs("Unexpected files found in extract directory (" +
                                                                 String.Join(",", unexpectedFiles.Select(d => d.FullName)) + 
                                                                 "). Pollution of extract directory is not permitted.", CheckResult.Fail));
            }
        }

        private bool IsValidPath(string path)
        {
            try
            {
                var fi = new FileInfo(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}