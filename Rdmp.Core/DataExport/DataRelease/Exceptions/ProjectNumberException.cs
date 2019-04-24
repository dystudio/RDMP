// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using Rdmp.Core.CatalogueLibrary.Data;
using Rdmp.Core.DataExport.Data;

namespace Rdmp.Core.DataExport.DataRelease.Exceptions
{
    /// <summary>
    /// Thrown when a given <see cref="IProject"/> doesn't have a <see cref="IProject.ProjectNumber"/> configured yet (null) or that number
    /// did not match an expected value (e.g. <see cref="ExternalCohortDefinitionData.ExternalProjectNumber"/>).
    /// </summary>
    public class ProjectNumberException : Exception
    {
        public ProjectNumberException(string s):base(s)
        {
            
        }
    }
}