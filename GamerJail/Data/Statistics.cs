using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace GamerJail.Data
{
    public class Statistics
    {
        public TimePeriod TimePeriod { get; set; }
        public TimeSpan TimePlayed { get; set; }
        public List<GameStatistic> Games { get; set; }
    }

    public class GameStatistic
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public TimeSpan TimePlayed { get; set; }
        public Guid Guid { get; set; }
        public Color ChartColor { get; set; }
    }

    public enum TimePeriod
    {
        [Description("Heute")]
        Day,
        [Description("Woche")]
        Week,
        [Description("Monat")]
        Month,
        [Description("Jahr")]
        Year,
        [Description("Insgesamt")]
        Ever
    }
}
