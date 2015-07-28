using System;
using Jugenschutzprogramm_Installer.Model;

namespace Jugenschutzprogramm_Installer.ViewManagement
{
    abstract class View : PropertyChangedBase
    {
        private bool _canGoForward = true;

        protected View(Setup config)
        {
            Setup = config;
        }

        public event EventHandler GoForwardChanged;

        public Setup Setup { get; set; }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set
            {
                if (SetProperty(value, ref _canGoForward))
                    GoForwardChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}