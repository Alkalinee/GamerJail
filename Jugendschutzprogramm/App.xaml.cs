using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using Jugendschutzprogramm.Logic;

namespace Jugendschutzprogramm
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private Window _window;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var tbi = new TaskbarIcon
            {
                IconSource =
                    new BitmapImage(new Uri(
                        @"pack://application:,,,/Jugendschutzprogramm;component/Resources/Icon.ico", UriKind.Absolute)),
                ToolTipText = "Jugenschutzprogramm"
            };
            tbi.TrayLeftMouseDown += Tbi_TrayLeftMouseDown;

            ServiceManager.Current.Load();
        }

        private void Tbi_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            if (_window != null)
            {
                _window.Activate();
            }
            else
            {
                _window = new MainWindow();
                _window.Closed += (s, o) => _window = null;
                _window.Show();
            }
        }
    }
}