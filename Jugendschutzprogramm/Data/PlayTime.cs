using System;

namespace Jugendschutzprogramm.Data
{
    class PlayTime
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid Program { get; set; }
        public Guid Guid { get; set; }
    }
}
