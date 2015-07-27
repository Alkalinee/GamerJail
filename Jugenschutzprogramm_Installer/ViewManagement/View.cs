using System;
using Jugenschutzprogramm.Shared;

namespace Jugenschutzprogramm_Installer.ViewManagement
{
    abstract class View : PropertyChangedBase
    {
        private RelayCommand _goForwardCommand;
        private RelayCommand _goBackCommand;

        protected View(Config config)
        {
            Config = config;
        }

        public event EventHandler GoForward;
        public event EventHandler GoBack;

        public Config Config { get; set; }
        public RelayCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand = new RelayCommand(parameter =>
                {
                    GoForward?.Invoke(this, EventArgs.Empty);
                }));
            }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(parameter =>
                {
                    GoBack?.Invoke(this, EventArgs.Empty);
                }));
            }
        }
    }
}