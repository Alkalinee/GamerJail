using System;
using System.Collections.Generic;

namespace GamerJail.Data
{
    class HistoryEntry
    {
        public DateTime DateTime { get; set; }
        public List<GameStatistic> Games { get; set; }
    }
}
