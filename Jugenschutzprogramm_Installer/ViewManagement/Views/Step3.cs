using System;
using System.IO;
using System.Security;
using System.Windows;
using Jugenschutzprogramm_Installer.Model;
using Ookii.Dialogs.Wpf;

namespace Jugenschutzprogramm_Installer.ViewManagement.Views
{
    class Step3 : View
    {
        private SecureString _password1;
        private SecureString _password2;
        private string _errorMessage;
        private RelayCommand _changeInstallationPathCommand;

        public Step3(Setup setup) : base(setup)
        {
            CanGoForward = false;
            setup.InstallationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Jugendschutzprogramm");
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(value, ref _errorMessage); }
        }

        public RelayCommand ChangeInstallationPathCommand
        {
            get
            {
                return _changeInstallationPathCommand ?? (_changeInstallationPathCommand = new RelayCommand(parameter =>
                {
                    var fbd = new VistaFolderBrowserDialog
                    {
                        RootFolder = Environment.SpecialFolder.ProgramFiles,
                        ShowNewFolderButton = true
                    };

                    if (fbd.ShowDialog() == true)
                    {
                        var directoryInfo = new DirectoryInfo(Path.Combine(fbd.SelectedPath, "Jugendschutzprogramm"));
                        if (directoryInfo.Exists)
                        {
                            if (directoryInfo.GetFileSystemInfos().Length > 0)
                            {
                                if (MessageBox.Show(
                                    "Der Ordner existiert bereits und enthält Dateien. Diese werden bei der Installation überschrieben. Forfahren?",
                                    "Pfad existiert bereits", MessageBoxButton.OKCancel, MessageBoxImage.Warning) ==
                                    MessageBoxResult.Cancel)
                                    return;
                            }
                            else
                            {
                                if (MessageBox.Show(
                                    "Der Ordner existiert bereits. Forfahren?",
                                    "Pfad existiert bereits", MessageBoxButton.OKCancel, MessageBoxImage.Warning) ==
                                    MessageBoxResult.Cancel)
                                    return;
                            }
                        }
                        Setup.InstallationPath = directoryInfo.FullName;
                    }
                }));
            }
        }

        public void Password1Changed(SecureString passsword)
        {
            _password1 = passsword;
            RefreshPasswords();
        }

        public void Password2Changed(SecureString passsword)
        {
            _password2 = passsword;
            RefreshPasswords();
        }

        private void RefreshPasswords()
        {
            CanGoForward = CheckStrings(_password1, _password2);
            if (CanGoForward)
                Setup.Password = _password1;
        }

        private bool CheckStrings(SecureString s1, SecureString s2)
        {
            if (s1 == null || s2 == null)
            {
                ErrorMessage = null;
                return false;
            }

            if (s1.Length < 6)
            {
                ErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein";
                return false;
            }

            if (!s1.SecureStringEqual(s2))
            {
                ErrorMessage = "Die Passwörter stimmen nicht überein";
                return false;
            }
            
            ErrorMessage = null;
            return true;
        }
    }
}
