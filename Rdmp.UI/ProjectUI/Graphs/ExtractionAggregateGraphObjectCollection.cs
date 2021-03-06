// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using Rdmp.Core.Curation.Data.Aggregation;
using Rdmp.Core.Curation.Data.Dashboarding;
using Rdmp.Core.DataExport.Data;

namespace Rdmp.UI.ProjectUI.Graphs
{
    /// <summary>
    /// Persistence/instantiation collection for <see cref="ExtractionAggregateGraphUI"/>.  Records which <see cref="Graph"/> is being
    /// visualized (e.g. healthboards over time) with which extractable dataset in which extraction (<see cref="SelectedDataSets"/>)
    /// </summary>
    public class ExtractionAggregateGraphObjectCollection : PersistableObjectCollection
    {
        /// <summary>
        /// The extraction dataset (in a given <see cref="ExtractionConfiguration"/>) to which the <see cref="Graph"/> results
        /// should be limited.  The graph should only depict records appearing in this extract.
        /// </summary>
        public SelectedDataSets SelectedDataSets { get { return (SelectedDataSets) DatabaseObjects[0]; }}
        
        /// <summary>
        /// The graph to be shown
        /// </summary>
        public AggregateConfiguration Graph { get { return (AggregateConfiguration) DatabaseObjects[1]; }}


        /// <summary>
        /// Constructor used for persistence
        /// </summary>
        public ExtractionAggregateGraphObjectCollection()
        {

        }

        /// <summary>
        /// Use this constructor at runtime
        /// </summary>
        /// <param name="selectedDataSet"></param>
        /// <param name="graph"></param>
        public ExtractionAggregateGraphObjectCollection(SelectedDataSets selectedDataSet, AggregateConfiguration graph):this()
        {
            DatabaseObjects.Add(selectedDataSet);
            DatabaseObjects.Add(graph);
        }
    }
}