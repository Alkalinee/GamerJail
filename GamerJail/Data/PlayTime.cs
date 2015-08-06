using System;

namespace GamerJail.Data
{
    public class PlayTime
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid Program { get; set; }
        public Guid Guid { get; set; }
    }
}
