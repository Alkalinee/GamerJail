using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using GamerJail.Shared;
using Microsoft.Win32;

namespace GamerJail.Installer.Model
{
    class Setup : PropertyChangedBase
    {
        private string _installationPath;
        private double _currentProgress;
        private string _currentStatus;

        public Setup()
        {
            Config = new Config();
        }

        public event EventHandler InstallationFinished;

        public Config Config { get; set; }

        public string InstallationPath
        {
            get { return _installationPath; }
            set { SetProperty(value, ref _installationPath); }
        }

        public double CurrentProgress
        {
            get { return _currentProgress; }
            set { SetProperty(value, ref _currentProgress); }
        }

        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { SetProperty(value, ref _currentStatus); }
        }

        public async Task Begin()
        {
            CurrentStatus = "Dateien werden kopiert";

            var files = new[]
            {
                "EntityFramework.dll",
                "EntityFramework.SqlServer.dll",
                "EntityFramework.SqlServer.xml",
                "EntityFramework.xml",
                "GamerJail.exe",
                "GamerJail.exe.config",
                "GamerJail.pdb",
                "GamerJail.Resources.dll",
                "GamerJail.Resources.pdb",
                "GamerJail.Shared.dll",
                "GamerJail.Shared.pdb",
                "Hardcodet.Wpf.TaskbarNotification.dll",
                "Hardcodet.Wpf.TaskbarNotification.pdb",
                "Hardcodet.Wpf.TaskbarNotification.xml",
                "MahApps.Metro.dll",
                "MahApps.Metro.xml",
                "Microsoft.Expression.Drawing.dll",
                "Microsoft.Expression.Drawing.xml",
                "PieControls.dll",
                "PieControls.pdb",
                "System.Data.SQLite.dll",
                "System.Data.SQLite.EF6.dll",
                "System.Data.SQLite.Linq.dll",
                "System.Data.SQLite.xml",
                "System.Windows.Interactivity.dll",
                "x64/SQLite.Interop.dll",
                "x86/SQLite.Interop.dll"
            };

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var resourceName = $"pack://application:,,,/GamerJail.Installer;component/InstallationFiles/{file}";
                var fileInfo = new FileInfo(Path.Combine(InstallationPath, file.Replace("/", @"\")));
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                await Task.Run(() => WriteResourceToFile(resourceName, fileInfo.FullName));
                CurrentProgress = i/(double) files.Length;
            }

            CurrentProgress = 0.5;
            CurrentStatus = "Config-Datei wird generiert";

            var configFile =
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GamerJail", "config.xml"));
            if (!configFile.Directory.Exists)
                configFile.Directory.Create();

            File.WriteAllText(configFile.FullName
                , new JavaScriptSerializer().Serialize(Config));

            CurrentProgress += 0.25;

            CurrentStatus = "Autostarteintrag wird erstellt";
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key.SetValue("GamerJail", "\"" + Path.Combine(InstallationPath, "GamerJail.exe") + "\"");
            CurrentProgress = 1;

            await Task.Delay(500);
            InstallationFinished?.Invoke(this, EventArgs.Empty);
        }

        private static void WriteResourceToFile(string resourceName, string fileName)
        {
            var resource = Application.GetResourceStream(new Uri(resourceName));

            using (var stream = resource.Stream)
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(file);
                }
            }
        }
    }
}