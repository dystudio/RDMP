﻿// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using Rdmp.Core.Curation.Data.Pipelines;
using Rdmp.Core.DataFlowPipeline.Events;
using Rdmp.Core.Logging.Listeners;
using ReusableLibraryCode.Progress;
using System;

namespace Rdmp.Core.CommandLine.Runners
{
    /// <summary>
    /// Runner that executes a single <see cref="IPipeline"/> under the given <see cref="IPipelineUseCase"/>
    /// </summary>
    public interface IPipelineRunner : IRunner
    {
        /// <summary>
        /// Called when the pipeline has finished executing
        /// </summary>
        event PipelineEngineEventHandler PipelineExecutionFinishedsuccessfully;


        /// <summary>
        /// Adds an additional listener to report events to when the runner is executed
        /// </summary>
        /// <param name="toAdd"></param>
        void SetAdditionalProgressListener(IDataLoadEventListener toAdd);
    }
}