// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using CatalogueManager.CommandExecution.AtomicCommands;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using DataExportManager.CohortUI;
using Rdmp.Core.DataExport.Data.DataTables;
using Rdmp.Core.DataExport.Providers.Nodes.UsedByProject;
using ReusableLibraryCode.CommandExecution.AtomicCommands;
using ReusableLibraryCode.Icons.IconProvision;

namespace DataExportManager.CommandExecution.AtomicCommands
{
    internal class ExecuteCommandShowSummaryOfCohorts : BasicUICommandExecution,IAtomicCommand
    {
        private readonly string _commandName;
        private readonly ExtractableCohort[] _onlyCohorts;

        public ExecuteCommandShowSummaryOfCohorts(IActivateItems activator)
            : base(activator)
        {
            _commandName = "Show Detailed Summary of Cohorts";
        }

        public ExecuteCommandShowSummaryOfCohorts(IActivateItems activator,CohortSourceUsedByProjectNode projectSource) : base(activator)
        {
            _commandName = "Show Detailed Summary of Project Cohorts";

            if (projectSource.IsEmptyNode)
                SetImpossible("Node is empty");
            else
                _onlyCohorts = projectSource.CohortsUsed.Select(u => u.ObjectBeingUsed).ToArray();
        }

        [ImportingConstructor]
        public ExecuteCommandShowSummaryOfCohorts(IActivateItems activator, ExternalCohortTable externalCohortTable) : base(activator)
        {
            _commandName = "Show Detailed Summary of Cohorts";
            _onlyCohorts = activator.CoreChildProvider.GetChildren(externalCohortTable).OfType<ExtractableCohort>().ToArray();
        }

        public override string GetCommandHelp()
        {
            return "Show information about the cohort lists stored in your cohort database (number of patients etc)";
        }

        public override string GetCommandName()
        {
            return _commandName;
        }

        public override void Execute()
        {
            var extractableCohortCollection = new ExtractableCohortCollectionUI();
            extractableCohortCollection.SetItemActivator(Activator);
            Activator.ShowWindow(extractableCohortCollection, true);

            if (_onlyCohorts != null)
                extractableCohortCollection.SetupFor(_onlyCohorts);
            else
                extractableCohortCollection.SetupForAllCohorts(Activator);
        }

        public override Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.AllCohortsNode);
        }
    }
}