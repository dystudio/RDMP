// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Data.Common;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;

namespace Rdmp.Core.Logging.PastEvents
{
    /// <summary>
    /// Readonly audit of a historical logged event which was noteworthy during the logged activity (See ArchivalDataLoadInfo)
    /// </summary>
    public class ArchivalProgressLog : IArchivalLoggingRecordOfPastEvent, IComparable, IHasSummary
    {
        public int ID { get; internal set; }
        public DateTime Date { get; internal set; }
        public string EventType { get; internal set; }
        public string Description { get; internal set; }

        public ArchivalProgressLog(DbDataReader r)
        {
            ID = (int)r["ID"];

            if (r["time"] != DBNull.Value)
                Date = Convert.ToDateTime(r["time"]);

            EventType = r["eventType"] as string;
            Description = r["description"] as string;
        }
        public override string ToString()
        {
            return Date + " - " + Description;
        }

        public int CompareTo(object obj)
        {
            var other = obj as ArchivalProgressLog;
            if (other != null)
                if (Date == other.Date)
                    return 0;
                else
                    return Date > other.Date ? 1 : -1;

            return System.String.Compare(ToString(), obj.ToString(), System.StringComparison.Ordinal);
        }

        public void GetSummary(out string title, out string body,out string stackTrace, out CheckResult level)
        {
            level = EventType == "OnWarning"? CheckResult.Warning : CheckResult.Success;
            title = Date.ToString();
            body = Description;
            stackTrace = null;
        }
    }
}