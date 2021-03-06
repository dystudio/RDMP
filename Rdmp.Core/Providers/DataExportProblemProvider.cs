// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using Rdmp.Core.DataExport.Data;
using Rdmp.Core.Providers.Nodes;
using Rdmp.Core.Providers.Nodes.ProjectCohortNodes;

namespace Rdmp.Core.Providers
{
    /// <summary>
    /// Identifies all rapidly detectable problems with the configurations of Data Export items
    /// </summary>
    public class DataExportProblemProvider:ProblemProvider
    {
        private DataExportChildProvider _exportChildProvider;
        
        /// <inheritdoc/>
        public override void RefreshProblems(ICoreChildProvider childProvider)
        {
            _exportChildProvider = childProvider as DataExportChildProvider;
        }


        /// <inheritdoc/>
        protected override string DescribeProblemImpl(object o)
        {
            if (o is Project)
                return DescribeProblem((Project) o);

            if (o is ProjectSavedCohortsNode)
                return DescribeProblem((ProjectSavedCohortsNode)o);

            if (o is ExtractionConfigurationsNode)
                return DescribeProblem((ExtractionConfigurationsNode) o);

            if (o is SelectedDataSets)
                return DescribeProblem((SelectedDataSets)o);

            if (o is ExtractionConfiguration)
                return DescribeProblem((ExtractionConfiguration) o);

            if (o is ExternalCohortTable)
                return DescribeProblem((ExternalCohortTable) o);

            return null;
        }

        private string DescribeProblem(ExternalCohortTable externalCohortTable)
        {
            if (_exportChildProvider != null && _exportChildProvider.BlackListedSources.Contains(externalCohortTable))
                return "Cohort Source database was unreachable";

            return null;
        }

        private string DescribeProblem(ExtractionConfiguration extractionConfiguration)
        {
            if (extractionConfiguration.Cohort_ID == null)
                return "Configuration has no Cohort configured";

            if (_exportChildProvider != null)
                if (!_exportChildProvider.GetDatasets(extractionConfiguration).Any()) //there are no selected datasets!
                    return "Configuration has no selected datasets";

            return null;
        }

        private string DescribeProblem(SelectedDataSets selectedDataSets)
        {
            if (_exportChildProvider.IsMissingExtractionIdentifier(selectedDataSets))
                return "There are no IsExtractionIdentifier columns in dataset";

            return null;
        }

        private string DescribeProblem(ExtractionConfigurationsNode extractionConfigurationsNode)
        {
            if (_exportChildProvider.Projects.Contains(extractionConfigurationsNode.Project))
                if (!_exportChildProvider.GetConfigurations(extractionConfigurationsNode.Project).Any())
                    return "Project has no ExtractionConfigurations";

            return null;
        }

        private string DescribeProblem(ProjectSavedCohortsNode projectSavedCohortsNode)
        {
            if (_exportChildProvider.ProjectHasNoSavedCohorts(projectSavedCohortsNode.Project))
                return "Project has no cohorts";

            return null;
        }


        private string DescribeProblem(Project project)
        {
            if (project.ProjectNumber == null)
                return "Project has no ProjectNumber";

            if (string.IsNullOrWhiteSpace(project.ExtractionDirectory))
                return "Project has no Extraction Directory configured";

            return null;
        }
    }
}
