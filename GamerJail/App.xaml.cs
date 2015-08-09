using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Views;
using Hardcodet.Wpf.TaskbarNotification;

namespace GamerJail
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private Window _window;

        public TaskbarIcon Icon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Contains("/uninstall"))
            {
                new UninstallWindow().Show();
                return;
            }

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Icon = new TaskbarIcon
            {
                IconSource =
                    new BitmapImage(new Uri(
                        @"pack://application:,,,/GamerJail;component/Resources/Icon.ico", UriKind.Absolute)),
                ToolTipText = "GamerJail"
            };
            Icon.TrayLeftMouseDown += Tbi_TrayLeftMouseDown;

            ServiceManager.Current.Load(Icon);

            if (ServiceManager.Current.Config.ProtectionLevel == ProtectionLevel.Nothing)
            {
                Icon.ContextMenu = new ContextMenu();
                var item = new MenuItem { Header = "Beenden" };
                item.Click += (sender, args) => Current.Shutdown();
                Icon.ContextMenu.Items.Add(item);
            }

            if (e.Args.Contains("/firstStart"))
                Icon.ShowBalloonTip("GamerJail",
                    "GamerJail wurde gestartet und ist nun aktiv.", BalloonIcon.Info);

            Current.Exit += Current_Exit;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            Icon.Dispose();
            ServiceManager.Current.DatabaseManager.Dispose();
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

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnExceptionOccurred((Exception)e.ExceptionObject);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            OnExceptionOccurred(e.Exception);
        }

        private void OnExceptionOccurred(Exception exception)
        {
            File.AppendAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GamerJail",
                    "log.txt"), $"{DateTime.Now}\r\n------------------------------------------\r\n{exception}");
        }
    }
}