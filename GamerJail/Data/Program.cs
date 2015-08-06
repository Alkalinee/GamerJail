using System;
using System.Windows.Media;
using GamerJail.Utilities;

namespace GamerJail.Data
{
    public class Program
    {
        private ImageSource _icon;

        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsGame { get; set; }
        public Guid Guid { get; set; }
        public DateTime Timestamp { get; set; }

        public string Filename => System.IO.Path.GetFileNameWithoutExtension(Path);
        public ImageSource Icon => _icon ?? (_icon = Path.GetFileImage());
    }
}
