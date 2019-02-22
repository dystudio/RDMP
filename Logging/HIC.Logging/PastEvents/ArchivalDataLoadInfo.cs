// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using FAnsi.Discovery;

namespace HIC.Logging.PastEvents
{
    /// <summary>
    /// Readonly historical version of DataLoadInfo.  The central hierarchical RDMP logging database records activites across all areas of the program in a central
    /// place.  You can process these records programatically via LogManager.  This class contains public properties for each of the sub concepts (Errors, Progress
    /// messages, Tables loaded etc).  See Logging.cd for more information
    /// </summary>
    public class ArchivalDataLoadInfo : IArchivalLoggingRecordOfPastEvent, IComparable
    {
        private readonly DiscoveredDatabase _loggingDatabase;

        public int ID { get; private set; }
        public int DataLoadTaskID { get; set; }
        public const int MaxDescriptionLength = 300;

        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public const int MaxChildrenToFetch = 1000;

        public bool HasErrors { get; private set; }

        public string ToShortString()
        {
            var s = ToString();
            if (s.Length > ArchivalDataLoadInfo.MaxDescriptionLength)
                return s.Substring(0, ArchivalDataLoadInfo.MaxDescriptionLength) + "...";
            return s;
        }

        public override string ToString()
        {
            string elapsed = "";
            if (EndTime != null)
            {
                var ts = EndTime.Value.Subtract(StartTime);
                elapsed = " (" + ts.ToString(@"hh\:mm\:ss")+ ")";
            }

            return Description + "(ID="+ID +") - " + StartTime + " - " + (EndTime != null ? EndTime.ToString() : "<DidNotFinish>") + elapsed;
        }

    
        /// <summary>
        /// All tables loaded during the run (up to <see cref="MaxChildrenToFetch"/>)
        /// </summary>
        public List<ArchivalTableLoadInfo>  TableLoadInfos { get { return _knownTableInfos.Value; }}
        /// <summary>
        /// All errors that occured during the run  (up to <see cref="MaxChildrenToFetch"/>)
        /// </summary>
        public List<ArchivalFatalError> Errors { get { return _knownErrors.Value; } }
        /// <summary>
        /// All progress messages recorded during the run (up to <see cref="MaxChildrenToFetch"/>)
        /// </summary>
        public List<ArchivalProgressLog> Progress { get { return _knownProgress.Value; } }

        readonly Lazy<List<ArchivalTableLoadInfo>> _knownTableInfos;
        readonly Lazy<List<ArchivalFatalError>> _knownErrors;
        readonly Lazy<List<ArchivalProgressLog>> _knownProgress;
        
        public string Description { get; set; }
        
        internal ArchivalDataLoadInfo(DbDataReader r,DiscoveredDatabase loggingDatabase)
        {
            _loggingDatabase = loggingDatabase;
            ID = Convert.ToInt32(r["ID"]);
            DataLoadTaskID = Convert.ToInt32(r["dataLoadTaskID"]);
            
            //populate basic facts from the table
            StartTime = (DateTime)r["startTime"];
            if (r["endTime"] == null || r["endTime"] == DBNull.Value)
                EndTime = null;
            else
                EndTime = Convert.ToDateTime(r["endTime"]);

            Description = r["description"] as string;

            HasErrors = r["hasErrors"] != DBNull.Value;

            _knownTableInfos = new Lazy<List<ArchivalTableLoadInfo>>(GetTableInfos);
            _knownErrors = new Lazy<List<ArchivalFatalError>>(GetErrors);
            _knownProgress = new Lazy<List<ArchivalProgressLog>>(GetProgress);
        }


        public int CompareTo(object obj)
        {
            var other = obj as ArchivalDataLoadInfo;
            if (other != null)
                if (StartTime == other.StartTime)
                    return 0;
                else
                    return StartTime > other.StartTime ? 1 : -1;

            return System.String.Compare(ToString(), obj.ToString(), System.StringComparison.Ordinal);
        }

        private List<ArchivalTableLoadInfo> GetTableInfos()
        {
            List<ArchivalTableLoadInfo> toReturn = new List<ArchivalTableLoadInfo>();

            using (var con = _loggingDatabase.Server.GetConnection())
            {
                con.Open();

                var cmd =  _loggingDatabase.Server.GetCommand("SELECT * FROM TableLoadRun WHERE dataLoadRunID=" +ID , con);
                var r = cmd.ExecuteReader();

                while(r.Read())
                    toReturn.Add(new ArchivalTableLoadInfo(this,r,_loggingDatabase));
            }

            return toReturn;
        }

        private List<ArchivalProgressLog> GetProgress()
        {
            List<ArchivalProgressLog> toReturn = new List<ArchivalProgressLog>();

            using (var con = _loggingDatabase.Server.GetConnection())
            {
                con.Open();

                var cmd = _loggingDatabase.Server.GetCommand("SELECT * FROM ProgressLog WHERE dataLoadRunID=" + ID, con);
                var r = cmd.ExecuteReader();

                while (r.Read())
                    toReturn.Add(new ArchivalProgressLog(r));
            }

            return toReturn;
        }

        private List<ArchivalFatalError> GetErrors()
        {
            List<ArchivalFatalError> toReturn = new List<ArchivalFatalError>();

            using (var con = _loggingDatabase.Server.GetConnection())
            {
                con.Open();

                var cmd = _loggingDatabase.Server.GetCommand("SELECT * FROM FatalError WHERE dataLoadRunID=" + ID, con);
                var r = cmd.ExecuteReader();

                while (r.Read())
                    toReturn.Add(new ArchivalFatalError(r));
            }

            return toReturn;
        }
    }
}