// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using CatalogueLibrary.Data;
using CatalogueManager.ItemActivation;
using CatalogueManager.SimpleDialogs.Reports;
using ReusableLibraryCode.CommandExecution.AtomicCommands;
using ReusableLibraryCode.Icons.IconProvision;

namespace CatalogueManager.CommandExecution.AtomicCommands
{
    public class ExecuteCommandGenerateMetadataReport:BasicUICommandExecution,IAtomicCommand
    {
        private readonly ICatalogue _catalogue;

        public ExecuteCommandGenerateMetadataReport(IActivateItems activator,ICatalogue initialSelection):base(activator)
        {
            _catalogue = initialSelection;
        }

        public override void Execute()
        {
            base.Execute();

            MetadataReportUI dialog = new MetadataReportUI(Activator, _catalogue);
            dialog.Show();
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return null;
        }
    }
}