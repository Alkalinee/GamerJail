using System;
using System.Web.Script.Serialization;
using Jugenschutzprogramm.Shared;
using System.IO;

namespace Jugenschutzprogramm_Installer.Model
{
    class Setup : PropertyChangedBase
    {
        private string _installationPath;

        public Setup()
        {
            Config = new Config();
        }

        public Config Config { get; set; }

        public string InstallationPath
        {
            get { return _installationPath; }
            set { SetProperty(value, ref _installationPath); }
        }

        public void Install()
        {
            var configFile =
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Jugendschutzprogramm", "config.xml"));
            if (!configFile.Directory.Exists)
                configFile.Directory.Create();

            File.WriteAllText(configFile.FullName
                , new JavaScriptSerializer().Serialize(Config));
        }
    }
}