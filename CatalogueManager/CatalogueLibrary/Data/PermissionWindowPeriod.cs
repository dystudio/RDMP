using System;
using System.Xml;
using System.Xml.Serialization;

namespace CatalogueLibrary.Data
{
    /// <summary>
    /// Note all times are stored as UTC
    /// </summary>
    public class PermissionWindowPeriod
    {
        /// <summary>
        /// Which day this period is in (periods cannot cross day boundaries)
        /// </summary>
        public int DayOfWeek { get; set; }
        
        // XML Serialiser doesn't support TimeSpan :(

        /// <summary>
        /// The start time on the day in which the activity becomes allowable
        /// </summary>
        [XmlIgnore]
        public TimeSpan Start { get; set; }

        /// <summary>
        /// The end time on the day in which the activity is no longer allowed
        /// </summary>
        [XmlIgnore]
        public TimeSpan End { get; set; }

        #region Required for correct XML serialisation

        // These pollute the public API unfortunately, but are required
        /// <inheritdoc cref="Start"/>
        [XmlElement(DataType = "duration", ElementName = "Start")]
        public string StartString
        {
            get { return XmlConvert.ToString(Start); }
            set { Start = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value); }
        }

        /// <inheritdoc cref="End"/>
        [XmlElement(DataType = "duration", ElementName = "End")]
        public string EndString
        {
            get { return XmlConvert.ToString(End); }
            set { End = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value); }
        }

        /// <summary>
        /// Used by serialization only
        /// </summary>
        internal PermissionWindowPeriod()
        {
            // needed for serialisation
        }

        #endregion

        /// <summary>
        /// Defines a period of day during which an activity is allowable
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public PermissionWindowPeriod(int dayOfWeek, TimeSpan start, TimeSpan end)
        {
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
        }

        /// <summary>
        /// True if the <paramref name="timeToTest"/> falls with the allowable time range (and on the correct <see cref="DayOfWeek"/>)
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <param name="testToNearestSecond"></param>
        /// <returns></returns>
        public bool Contains(DateTime timeToTest, bool testToNearestSecond = false)
        {
            if ((int) timeToTest.DayOfWeek != DayOfWeek)
                return false;

            // If we are not testing to the nearest second, set the seconds var in the test to 0 so any Start and Ends defined without seconds are compared correctly
            var testTime = new TimeSpan(timeToTest.Hour, timeToTest.Minute, testToNearestSecond? timeToTest.Second : 0);
            return testTime >= Start && testTime <= End;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Start.ToString("hh':'mm") + "-" + End.ToString("hh':'mm");
        }
    }
}