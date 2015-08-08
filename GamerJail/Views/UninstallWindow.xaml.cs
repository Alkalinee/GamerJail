using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GamerJail.Logic;

namespace GamerJail.Views
{
    /// <summary>
    /// Interaktionslogik für UninstallWindow.xaml
    /// </summary>
    public partial class UninstallWindow
    {
        public UninstallWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ProgressBar.IsIndeterminate = true;
            StatusTextBlock.Text = "Beende offene Instanzen";
            var pid = Process.GetCurrentProcess().Id;
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == pid)
                    continue;

                if (process.ProcessName == "GamerJail")
                {
                    process.Kill();
                    await Task.Run(() => process.WaitForExit());
                }
            }

            StatusTextBlock.Text = "Lösche von Autostarteinträgen";
            await Task.Run(() => AutostartManager.Remove());
            await Task.Delay(500);

            if (RemoveUserSettingsRadioButton.IsChecked == true)
            {
                StatusTextBlock.Text = "Lösche Benutzerdaten";
                var userData =
                    new DirectoryInfo(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GamerJail"));
                if (userData.Exists)
                    await Task.Run(() => userData.Delete(true));
            }

            StatusTextBlock.Text = "Bye Bye";
            var file = new FileInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".bat"));
            var batchScript = $"@ECHO OFF\r\nping 127.0.0.1 > nul\r\necho j | @RD /S /Q \"{AppDomain.CurrentDomain.BaseDirectory}\"\r\necho j | del {file.Name}";

            await Task.Run(() => File.WriteAllText(file.FullName, batchScript));

            Process.Start(file.FullName);
            Application.Current.Shutdown();
        }
    }
}
