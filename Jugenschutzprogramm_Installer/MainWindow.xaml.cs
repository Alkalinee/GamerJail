using System.Windows;
using System.Windows.Input;

namespace Jugenschutzprogramm_Installer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += MainWindow_MouseDown;
            PART_CLOSE.Click += PART_CLOSE_Click;
            PART_MINIMIZE.Click += PART_MINIMIZE_Click;
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
          //  if (e.ChangedButton == MouseButton.Left)
            //    DragMove();
        }

        private void Buttonasd_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
