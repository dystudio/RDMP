// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using Rdmp.Core.Curation.Data.Pipelines;
using Rdmp.Core.Icons.IconProvision;

namespace Rdmp.UI.Icons.IconProvision.StateBasedIconProviders
{
    public class PipelineComponentStateBasedIconProvider : IObjectStateBasedIconProvider
    {
        private Bitmap _component;
        private Bitmap _soure;
        private Bitmap _destnition;

        public PipelineComponentStateBasedIconProvider()
        {
            _component = CatalogueIcons.PipelineComponent;
            _soure = CatalogueIcons.PipelineComponentSource;
            _destnition = CatalogueIcons.PipelineComponentDestination;
        }
        public Bitmap GetImageIfSupportedObject(object o)
        {
            if (o is PipelineComponent pc)
            {
                if (pc.Class != null && pc.Class.EndsWith("Source"))
                    return _soure;
                if (pc.Class != null && pc.Class.EndsWith("Destination"))
                    return _destnition;

                return _component;
            }

            return null;

        }
    }
}