// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using Rdmp.Core.CommandExecution;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.CommandExecution.Combining;
using Rdmp.Core.DataExport.Data;
using Rdmp.Core.Providers;
using Rdmp.UI.CommandExecution.AtomicCommands;
using Rdmp.UI.ItemActivation;
using Rdmp.UI.ProjectUI;

namespace Rdmp.UI.CommandExecution.Proposals
{
    class ProposeExecutionWhenTargetIsExtractionConfiguration:RDMPCommandExecutionProposal<ExtractionConfiguration>
    {
        public ProposeExecutionWhenTargetIsExtractionConfiguration(IActivateItems itemActivator) : base(itemActivator)
        {
        }

        public override bool CanActivate(ExtractionConfiguration target)
        {
            return !target.IsReleased;
        }

        public override void Activate(ExtractionConfiguration target)
        {
            if (!target.IsReleased)
                ItemActivator.Activate<ExecuteExtractionUI, ExtractionConfiguration>(target);
        }

        public override ICommandExecution ProposeExecution(ICombineToMakeCommand cmd, ExtractionConfiguration targetExtractionConfiguration, InsertOption insertOption = InsertOption.Default)
        {
            //user is trying to set the cohort of the configuration
            if (cmd is CatalogueCombineable sourceCatalogueCombineable)
            {
                var dataExportChildProvider = (DataExportChildProvider)ItemActivator.CoreChildProvider;
                var eds = dataExportChildProvider.ExtractableDataSets.SingleOrDefault(ds => ds.Catalogue_ID == sourceCatalogueCombineable.Catalogue.ID);

                if (eds == null)
                    return new ImpossibleCommand("Catalogue is not Extractable");
                
                return new ExecuteCommandAddDatasetsToConfiguration(ItemActivator, eds,targetExtractionConfiguration);   
            }

            if (cmd is ExtractableCohortCombineable sourceExtractableCohortCombineable)
                return new ExecuteCommandAddCohortToExtractionConfiguration(ItemActivator, sourceExtractableCohortCombineable, targetExtractionConfiguration);

            //user is trying to add datasets to a configuration
            var sourceExtractableDataSetCommand = cmd as ExtractableDataSetCombineable;

            if (sourceExtractableDataSetCommand != null)
                return new ExecuteCommandAddDatasetsToConfiguration(ItemActivator, sourceExtractableDataSetCommand, targetExtractionConfiguration);

            return null;
        }
    }
}
