using System;

namespace Jugendschutzprogramm.Data
{
    class Program
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsGame { get; set; }
        public Guid Guid { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
