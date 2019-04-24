// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using System.Windows.Forms;
using CatalogueManager.DataLoadUIs.LoadMetadataUIs;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using Rdmp.Core.CatalogueLibrary.Data.DataLoad;
using ReusableLibraryCode.CommandExecution.AtomicCommands;
using ReusableLibraryCode.Icons.IconProvision;

namespace CatalogueManager.CommandExecution.AtomicCommands
{
    internal class ExecuteCommandChooseLoadDirectory : BasicUICommandExecution, IAtomicCommand
    {
        private readonly LoadMetadata _loadMetadata;

        public ExecuteCommandChooseLoadDirectory(IActivateItems activator, LoadMetadata loadMetadata) : base(activator)
        {
            _loadMetadata = loadMetadata;
        }

        public override string GetCommandHelp()
        {
            return "Changes the load location\\working directory for the DLE load configuration";
        }

        public override void Execute()
        {
            base.Execute();

            var dialog = new ChooseLoadDirectoryUI(_loadMetadata);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _loadMetadata.LocationOfFlatFiles = dialog.Result.RootPath.FullName;
                _loadMetadata.SaveToDatabase();
                Publish(_loadMetadata);
            }
        }

        public override Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.LoadDirectoryNode);
        }
    }
}