using System.Windows;
using System.Windows.Controls;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Shared.Utilities;

namespace GamerJail.ViewModels
{
    class PasswordDialogViewModel : PropertyChangedBase
    {
        private RelayCommand _checkPasswordCommand;
        private readonly Window _window;
        private RelayCommand _closeCommand;

        public PasswordDialogViewModel(Window window)
        {
            _window = window;
        }

        public RelayCommand CheckPasswordCommand
        {
            get
            {
                return _checkPasswordCommand ?? (_checkPasswordCommand = new RelayCommand(parameter =>
                {
                    if (((PasswordBox)parameter).SecurePassword.SecureStringEqual(ServiceManager.Current.Config.Password))
                    {
                        _window.DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Das Passwort ist falsch. Versuchen Sie es erneut.", "Fehler");
                    }
                }));
            }
        }

        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand(parameter =>
                {
                    _window.Close();
                }));
            }
        }
    }
}
