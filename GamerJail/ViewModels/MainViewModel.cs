using System;
using System.Windows;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.ViewManagement;
using GamerJail.ViewModels.Pages;
using GamerJail.Views;

namespace GamerJail.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private RelayCommand _openConfigurationCommand;
        private IView _currentView;
        private bool _canGoBack;
        private RelayCommand _goBackCommand;
        private bool _isConfigurationOpen;
        private RelayCommand _openStatisticCommand;

        public MainViewModel()
        {
            ServiceManager = ServiceManager.Current;
            CurrentView = new HomeViewModel(ServiceManager);
        }

        public ServiceManager ServiceManager { get; set; }

        public IView CurrentView
        {
            get { return _currentView; }
            set
            {
                var oldView = _currentView;
                if (SetProperty(value, ref _currentView))
                {
                    oldView?.Close();
                    if (value != null)
                        value.CloseRequest += ViewCloseRequest;
                    if (oldView != null)
                        oldView.CloseRequest -= ViewCloseRequest;
                }
            }
        }

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(value, ref _canGoBack); }
        }

        private void ViewCloseRequest(object sender, EventArgs eventArgs)
        {
            CurrentView = new HomeViewModel(ServiceManager);
        }

        public RelayCommand OpenConfigurationCommand
        {
            get
            {
                return _openConfigurationCommand ?? (_openConfigurationCommand = new RelayCommand(parameter =>
                {
                    var passwordWindow = new PasswordDialogWindow {Owner = Application.Current.MainWindow};
                    if (passwordWindow.ShowDialog() != true)
                        return;

                    CurrentView = new ConfigurationViewModel(ServiceManager);
                    CanGoBack = true;
                    IsConfigurationOpen = true;
                }));
            }
        }

        public RelayCommand OpenStatisticCommand
        {
            get
            {
                return _openStatisticCommand ?? (_openStatisticCommand = new RelayCommand(parameter =>
                {
                    CanGoBack = true;
                    CurrentView = new StatisticViewModel(ServiceManager);
                }));
            }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(parameter =>
                {
                    CurrentView = new HomeViewModel(ServiceManager);
                    CanGoBack = false;
                    IsConfigurationOpen = false;
                }));
            }
        }

        public bool IsConfigurationOpen
        {
            get { return _isConfigurationOpen; }
            set { SetProperty(value, ref _isConfigurationOpen); }
        }
    }
}