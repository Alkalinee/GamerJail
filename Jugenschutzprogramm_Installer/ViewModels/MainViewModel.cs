using System.Collections.Generic;
using System.Linq;
using Jugenschutzprogramm.Shared;
using Jugenschutzprogramm_Installer.ViewManagement;
using Jugenschutzprogramm_Installer.ViewManagement.Views;

namespace Jugenschutzprogramm_Installer.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private View _currentView;
        private readonly List<View> _views;
        private bool _canGoBack;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private string _animation = "MoveForwardState";
        private Config _config;

        public MainViewModel()
        {
            _config = new Config();
            _views = new List<View> {new WelcomeView(_config), new Step1(_config), new Step2(_config)};
            CurrentView = _views.First();
        }

        public View CurrentView
        {
            get { return _currentView; }
            set
            {
                SetProperty(value, ref _currentView);
            }
        }

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(value, ref _canGoBack); }
        }

        public string Animation
        {
            get { return _animation; }
            set { SetProperty(value, ref _animation); }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(parameter =>
                {
                    Animation = "MoveBackwardState";
                    CurrentView = _views[_views.IndexOf(CurrentView) - 1];
                    CanGoBack = _views.IndexOf(CurrentView) > 0;
                }));
            }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand = new RelayCommand(parameter =>
                {
                    Animation = "MoveForwardState";
                    CurrentView = _views[_views.IndexOf(CurrentView) + 1];
                    CanGoBack = true;
                }));
            }
        }
    }
}