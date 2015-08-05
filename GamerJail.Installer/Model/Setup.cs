using System;
using System.Web.Script.Serialization;
using GamerJail.Shared;
using System.IO;

namespace GamerJail.Installer.Model
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
                    "GamerJail", "config.xml"));
            if (!configFile.Directory.Exists)
                configFile.Directory.Create();

            File.WriteAllText(configFile.FullName
                , new JavaScriptSerializer().Serialize(Config));
        }
    }
}