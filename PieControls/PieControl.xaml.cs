using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieControls
{
    /// <summary>
    /// Interaction logic for PieControl.xaml
    /// </summary> 
    public partial class PieControl
    {
        readonly Dictionary<Path, PieSegment> _pathDictionary = new Dictionary<Path, PieSegment>();
        public static readonly DependencyProperty PopupBrushProperty = DependencyProperty.Register("PopupBrush", typeof(Brush), typeof(PieControl));

        public Brush PopupBrush 
        {
            get 
            {
                return (Brush)GetValue(PopupBrushProperty);
            }
            set 
            {
                SetValue(PopupBrushProperty, value);
            } 
        }

        public PieControl()
        {
            DataContext = this;
            PopupBrush = Brushes.White;
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof (ObservableCollection<PieSegment>), typeof (PieControl), new PropertyMetadata(default(ObservableCollection<PieSegment>), DataPropertyChangedCallback));

        private static void DataPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pieControl = dependencyObject as PieControl;
            if (dependencyPropertyChangedEventArgs.NewValue == null)
                return;

            pieControl?.DataChanged((ObservableCollection<PieSegment>) dependencyPropertyChangedEventArgs.NewValue);
        }

        private void DataChanged(ObservableCollection<PieSegment> values)
        {
            values.CollectionChanged += values_CollectionChanged;
            foreach (var v in values)
            {
                v.PropertyChanged += pieSegment_PropertyChanged;
            }
            ResetPie();
        }

        public ObservableCollection<PieSegment> Data
        {
            get { return (ObservableCollection<PieSegment>) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        void AddPathToDictionary(Path path, PieSegment ps)
        {
            _pathDictionary.Add(path, ps);
            path.MouseEnter += Path_MouseEnter;            
            path.MouseMove += Path_MouseMove;
        }

        void Path_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = Mouse.GetPosition(this);            
            piePopup.Margin = new Thickness(point.X - piePopup.ActualWidth / 4, point.Y - (18 + piePopup.ActualHeight), 0, 0);
        }

        private void PieControl_MouseLeave(object sender, MouseEventArgs e)
        {
            piePopup.Visibility = Visibility.Collapsed;
        }     

        void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            piePopup.Visibility = Visibility.Visible;
            PieSegment seg = _pathDictionary[sender as Path];
            popupData.Text = seg.Name + " : " + ((seg.Value / Data.GetTotal()) * 100).ToString("N2") + "%";
            Point point = Mouse.GetPosition(this);
            piePopup.Margin = new Thickness(point.X - piePopup.ActualWidth / 4, point.Y - (18 + piePopup.ActualHeight), 0, 0);
        }

        void ClearPathDictionary()
        {
            foreach (Path path in _pathDictionary.Keys)
            {
                path.MouseEnter -= Path_MouseEnter;
                path.MouseMove -= Path_MouseMove;
            }
            _pathDictionary.Clear();
        }

        void CreatePiePathAndGeometries()
        {
            ClearPathDictionary();
            drawingCanvas.Children.Clear();
            drawingCanvas.Children.Add(piePopup);
            if (Data != null)
            {   
                double total = Data.GetTotal();
                if (total > 0)
                {
                    double angle = -Math.PI / 2;
                    foreach (PieSegment ps in Data)
                    {
                        //PieSegment ps = Data[1];
                        Geometry geometry;
                        Path path = new Path();
                        if (ps.Value == total)
                        {
                            geometry = new EllipseGeometry(new Point(Width / 2, Height / 2), Width / 2, Height / 2);
                        }
                        else
                        {
                            geometry = new PathGeometry();
                            double x = Math.Cos(angle) * Width / 2 + Width / 2;
                            double y = Math.Sin(angle) * Height / 2 + Height / 2;
                            LineSegment lingeSeg = new LineSegment(new Point(x, y), true);
                            double angleShare = (ps.Value / total) * 360;
                            angle += DegreeToRadian(angleShare);
                            x = Math.Cos(angle) * Width / 2 + Width / 2;
                            y = Math.Sin(angle) * Height / 2 + Height / 2;
                            ArcSegment arcSeg = new ArcSegment(new Point(x, y), new Size(Width / 2, Height / 2), angleShare, angleShare > 180, SweepDirection.Clockwise, false);
                            LineSegment lingeSeg2 = new LineSegment(new Point(Width / 2, Height / 2), true);
                            PathFigure fig = new PathFigure(new Point(Width / 2, Height / 2), new PathSegment[] { lingeSeg, arcSeg, lingeSeg2 }, false);
                            ((PathGeometry)geometry).Figures.Add(fig);
                        }
                        path.Fill = ps.GradientBrush;
                        path.Data = geometry;
                        AddPathToDictionary(path, ps);
                        drawingCanvas.Children.Add(path);
                    }
                }
            }
        }

        void ResetPie()
        {
            Dispatcher.Invoke(new Action(CreatePiePathAndGeometries));
        }

        void pieSegment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetPie();
        }

        void values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetPie();
        }
     
        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
