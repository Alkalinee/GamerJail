using System.ComponentModel;
using System.Windows.Media;

namespace PieControls
{
    public class PieSegment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _name;
        double _value;
        Brush _gradientBrush;
        Brush _solidBrush;
        Color _color;

        public double Value
        {
            get 
            {
                return _value;
            }
            set 
            {
                _value = value;
                OnPropertyChanged(this, "Value");
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;                
                _gradientBrush = new LinearGradientBrush(MakeSecondColor(_color, 50), _color, 45);
                _solidBrush = new SolidColorBrush(_color);
                _gradientBrush.Freeze();
                _solidBrush.Freeze();
                OnPropertyChanged(this, "Color");
            }
        }

        //difference should be a maximum value of 100
        Color MakeSecondColor(Color color, uint difference)
        {
            difference = difference > 100 ? 100 : difference;
            byte r = GetNewColorByte(color.R, difference);
            byte g = GetNewColorByte(color.G, difference);
            byte b = GetNewColorByte(color.B, difference);
            return Color.FromRgb(r, g, b);
        }

        //This method ensures that bytes never overflow to avoid drastic change in color
        byte GetNewColorByte(byte oldByte, uint difference)
        {
            if (oldByte + difference > 255)
            {
                return (byte)(oldByte - difference);
            }
            else
            {
                return (byte)(oldByte + difference);
            }
        }

        public Brush GradientBrush => _gradientBrush;

        public Brush SolidBrush => _solidBrush;

        public string Name 
        { 
            get 
            { 
                return _name; 
            } 
            set 
            { 
                _name = value; 
                OnPropertyChanged(this, "Name"); 
            } 
        }

        private void OnPropertyChanged(object sender, string propertyName)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
