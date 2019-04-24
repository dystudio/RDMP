// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using CatalogueManager.CommandExecution.Proposals;
using CatalogueManager.ItemActivation;
using CatalogueManager.Copying.Commands;
using Rdmp.Core.DataExport.Providers.Nodes;
using ReusableLibraryCode.CommandExecution;
using ReusableUIComponents.CommandExecution;

namespace DataExportManager.CommandExecution.Proposals
{
    class ProposeExecutionWhenTargetIsProjectCataloguesNode : RDMPCommandExecutionProposal<ProjectCataloguesNode>
    {
        private ProposeExecutionWhenTargetIsProject _projectFunctionality;

        public ProposeExecutionWhenTargetIsProjectCataloguesNode(IActivateItems itemActivator) : base(itemActivator)
        {
            _projectFunctionality = new ProposeExecutionWhenTargetIsProject(itemActivator);
        }

        public override bool CanActivate(ProjectCataloguesNode target)
        {
            return false;
        }

        public override void Activate(ProjectCataloguesNode target)
        {
            
        }

        public override ICommandExecution ProposeExecution(ICommand cmd, ProjectCataloguesNode target, InsertOption insertOption = InsertOption.Default)
        {
            //use the same drop options as Project except for this one

            if (cmd is CohortIdentificationConfigurationCommand)
                return null;

            
            return _projectFunctionality.ProposeExecution(cmd, target.Project, insertOption);
        }
    }
}