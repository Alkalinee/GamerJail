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
        private bool _isViewOpen;
        private RelayCommand _goBackCommand;
        private RelayCommand _openStatisticCommand;
        private RelayCommand _openHistoryCommand;

        public MainViewModel()
        {
            ServiceManager = ServiceManager.Current;
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

        public bool IsViewOpen
        {
            get { return _isViewOpen; }
            set { SetProperty(value, ref _isViewOpen); }
        }

        private void ViewCloseRequest(object sender, EventArgs eventArgs)
        {
            CurrentView = null;
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
                    IsViewOpen = true;
                }));
            }
        }

        public RelayCommand OpenStatisticCommand
        {
            get
            {
                return _openStatisticCommand ?? (_openStatisticCommand = new RelayCommand(parameter =>
                {
                    IsViewOpen = true;
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
                    CurrentView = null;
                    IsViewOpen = false;
                }));
            }
        }

        public RelayCommand OpenHistoryCommand
        {
            get
            {
                return _openHistoryCommand ?? (_openHistoryCommand = new RelayCommand(parameter =>
                {
                    CurrentView = new HistoryViewModel(ServiceManager);
                    IsViewOpen = true;
                }));
            }
        }
    }
}