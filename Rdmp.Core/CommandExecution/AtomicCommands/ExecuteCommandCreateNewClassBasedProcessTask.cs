// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.DataLoad;
using Rdmp.Core.DataLoad.Engine;
using Rdmp.Core.DataLoad.Engine.Attachers;
using Rdmp.Core.DataLoad.Engine.DataProvider;
using Rdmp.Core.DataLoad.Engine.Mutilators;

namespace Rdmp.Core.CommandExecution.AtomicCommands
{
    public class ExecuteCommandCreateNewClassBasedProcessTask : BasicCommandExecution
    {
        private readonly LoadMetadata _loadMetadata;
        private readonly LoadStage _loadStage;
        private readonly Type _type;
        private readonly ProcessTaskType _processTaskType;

        public ExecuteCommandCreateNewClassBasedProcessTask(IBasicActivateItems activator, LoadMetadata loadMetadata, LoadStage loadStage, 
            [DemandsInitialization("Class to execute, must be an attacher, mutilater etc", TypeOf = typeof(IDisposeAfterDataLoad))]
            Type type) : base(activator)
        {
            _loadMetadata = loadMetadata;
            _loadStage = loadStage;
            _type = type;

            if (typeof(IAttacher).IsAssignableFrom(_type))
                _processTaskType = ProcessTaskType.Attacher;
            else
            if (typeof(IDataProvider).IsAssignableFrom(_type))
                _processTaskType = ProcessTaskType.DataProvider;
            else
            if (typeof(IMutilateDataTables).IsAssignableFrom(_type))
                _processTaskType = ProcessTaskType.MutilateDataTable;
            else
            {
                SetImpossible($"Type '{_type}' was not a compatible one e.g. IAttacher, IDataProvider or IMutilateDataTables");
            }

        }

        public override void Execute()
        {
            ProcessTask newTask = new ProcessTask(BasicActivator.RepositoryLocator.CatalogueRepository,_loadMetadata, _loadStage);
            newTask.Path = _type.FullName;
            newTask.ProcessTaskType = _processTaskType;
            newTask.Name = _type.Name;
            newTask.SaveToDatabase();

            newTask.CreateArgumentsForClassIfNotExists(_type);

            Publish(_loadMetadata);
            Activate(newTask);
        }
    }
}