using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GamerJail.Installer.Model;
using GamerJail.Installer.ViewManagement;
using GamerJail.Installer.ViewManagement.Views;
using GamerJail.Shared;

namespace GamerJail.Installer.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private View _currentView;
        private readonly List<View> _views;
        private bool _canGoBack;
        private bool _canGoForward;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private string _animation = "MoveForwardState";
        private readonly Setup _setup;
        private bool _isFinalStep;

        public MainViewModel()
        {
            _setup = new Setup();
            _views = new List<View>
            {
                new WelcomeView(_setup),
                new Step1(_setup),
                new Step2(_setup),
                new Step3(_setup)
            };
            CurrentView = _views.First();

            _views.ForEach(view => view.GoForwardChanged += (sender, args) => RefreshCanGoForward());
            RefreshCanGoForward();
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

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set { SetProperty(value, ref _canGoForward); }
        }

        public string Animation
        {
            get { return _animation; }
            set { SetProperty(value, ref _animation); }
        }

        public bool IsFinalStep
        {
            get { return _isFinalStep; }
            set { SetProperty(value, ref _isFinalStep); }
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
                    RefreshCanGoForward();
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
                    if (IsFinalStep)
                    {
                        _setup.Install();
                        MessageBox.Show("Installiert!");
                        Application.Current.Shutdown();
                        return;
                    }
                    CurrentView = _views[_views.IndexOf(CurrentView) + 1];
                    CanGoBack = true;
                    RefreshCanGoForward();
                }));
            }
        }

        private void RefreshCanGoForward()
        {
            CanGoForward = CurrentView.CanGoForward;
            IsFinalStep = _views.IndexOf(CurrentView) == _views.Count - 1;
        }
    }
}