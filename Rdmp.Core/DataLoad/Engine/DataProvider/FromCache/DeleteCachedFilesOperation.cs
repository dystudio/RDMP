// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using Rdmp.Core.DataLoad.Engine.Job.Scheduling;
using ReusableLibraryCode.Progress;

namespace Rdmp.Core.DataLoad.Engine.DataProvider.FromCache
{
    /// <summary>
    /// UpdateProgressIfLoadsuccessful (See UpdateProgressIfLoadsuccessful) which also deletes files in the ForLoading directory that were generated during the
    /// load e.g. by a CachedFileRetriever.  Files are only deleted if the ExitCodeType.Success otherwise they are left in ForLoading for debugging / inspection.
    /// </summary>
    public class DeleteCachedFilesOperation : UpdateProgressIfLoadsuccessful
    {
        private readonly Dictionary<DateTime, FileInfo> _cacheFileMappings;

        public DeleteCachedFilesOperation(ScheduledDataLoadJob job, Dictionary<DateTime, FileInfo> cacheFileMappings)
            : base(job)
        {
            _cacheFileMappings = cacheFileMappings;
        }

        override public void LoadCompletedSoDispose(ExitCodeType exitCode, IDataLoadEventListener postLoadEventListener)
        {
            if (exitCode != ExitCodeType.Success)
                return;

            base.LoadCompletedSoDispose(exitCode, postLoadEventListener);

            foreach (KeyValuePair<DateTime, FileInfo> keyValuePair in _cacheFileMappings)
            {
                if (keyValuePair.Value == null)
                    continue;

                try
                {
                    keyValuePair.Value.Delete();
                }
                catch (IOException e)
                {
                    Job.LogWarning(GetType().FullName, "Could not delete cached file " + keyValuePair.Value + " (" + e.Message + ")make sure to delete it manually otherwise Schedule and file system will be desynched");
                }
            }
        }
    }
}