using System;
using System.Security;
using System.Windows;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Shared.Utilities;
using GamerJail.Views;

namespace GamerJail.ViewModels
{
    class ConfigurationViewModel : PropertyChangedBase
    {
        private readonly Window _window;
        private int _lowerValue;
        private int _upperValue;
        private bool _showWarning;
        private TimeSpan _gamingTimePerDay;
        private SecureString _password2;
        private SecureString _password1;
        private string _passwordErrorMessage;
        private bool _canChangePassword;
        private RelayCommand _changePasswordCommand;

        public ConfigurationViewModel(ServiceManager serviceManager, Window window)
        {
            _window = window;
            ServiceManager = serviceManager;
            GamingTimePerDay = serviceManager.Config.GamingTimePerDay;
            LowerValue = ConvertTimeSpanBack(serviceManager.Config.TimeSpan.FromTime);
            UpperValue = ConvertTimeSpanBack(serviceManager.Config.TimeSpan.ToTime);
        }

        private int ConvertTimeSpanBack(double time)
        {
            if(time < 4)
            {
                return (int)((time + 24) * 2) - 4 * 2;
            }
            else
            {
                return (int)(time * 2) - 4 * 2;
            }
        }

        public event EventHandler ClearPasswords;

        public ServiceManager ServiceManager { get; }

        public int LowerValue
        {
            get { return _lowerValue; }
            set
            {
                if (_lowerValue != value)
                {
                    _lowerValue = value;
                    var timeSpan = TimeSpan.FromMinutes(value * 30 + 4 * 60);
                    ServiceManager.Config.TimeSpan.FromTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    CheckIfEverythingIsAwesome();
                }
            }
        }

        public int UpperValue
        {
            get { return _upperValue; }
            set
            {
                if (_upperValue != value)
                {
                    _upperValue = value;
                    var timeSpan = TimeSpan.FromMinutes(value * 30 + 4 * 60);
                    ServiceManager.Config.TimeSpan.ToTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    CheckIfEverythingIsAwesome();
                }
            }
        }

        public bool ShowWarning
        {
            get { return _showWarning; }
            set { SetProperty(value, ref _showWarning); }
        }

        public TimeSpan GamingTimePerDay
        {
            get { return _gamingTimePerDay; }
            set
            {
                if (_gamingTimePerDay != value)
                {
                    _gamingTimePerDay = value;
                    ServiceManager.Config.GamingTimePerDay = value;
                    CheckIfEverythingIsAwesome();
                }
            }
        }

        public string PasswordErrorMessage
        {
            get { return _passwordErrorMessage; }
            set { SetProperty(value, ref _passwordErrorMessage); }
        }

        public bool CanChangePassword
        {
            get { return _canChangePassword; }
            set { SetProperty(value, ref _canChangePassword); }
        }

        public RelayCommand ChangePasswordCommand
        {
            get
            {
                return _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand(parameter =>
                {
                    var dialog = new PasswordDialogWindow {Owner = _window};
                    if (dialog.ShowDialog() == true)
                    {
                        ServiceManager.Config.Password = _password1;
                        MessageBox.Show("Das Passwort wurde erfolgreich geändert.", "Erfolgreich");
                    }
                    ClearPasswords?.Invoke(this, EventArgs.Empty);
                    PasswordErrorMessage = null;
                    _password1 = null;
                    _password2 = null;
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

        private void CheckIfEverythingIsAwesome()
        {
            ShowWarning = (UpperValue * 30 - LowerValue * 30) < ServiceManager.Config.GamingTimePerDay.TotalMinutes;
        }

        private void RefreshPasswords()
        {
            CanChangePassword = CheckStrings(_password1, _password2);
        }

        private bool CheckStrings(SecureString s1, SecureString s2)
        {
            if (s1 == null || s2 == null)
            {
                PasswordErrorMessage = null;
                return false;
            }

            if (s1.Length < 6)
            {
                PasswordErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein";
                return false;
            }

            if (!s1.SecureStringEqual(s2))
            {
                PasswordErrorMessage = "Die Passwörter stimmen nicht überein";
                return false;
            }

            PasswordErrorMessage = null;
            return true;
        }
    }
}
