using System.Security;
using Jugenschutzprogramm.Shared;

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
        public SecureString Password { get; set; }

        public string InstallationPath
        {
            get { return _installationPath; }
            set { SetProperty(value, ref _installationPath); }
        }
    }
}
