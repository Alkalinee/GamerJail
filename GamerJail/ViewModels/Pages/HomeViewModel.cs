using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.ViewManagement;

namespace GamerJail.ViewModels.Pages
{
    class HomeViewModel : PropertyChangedBase, IView
    {
        private readonly DispatcherTimer _dispatcherTimer;

        public HomeViewModel()
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            ServiceManager = ServiceManager.Current;
            _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
            Application.Current.Windows.Cast<Window>().First().Closed += (sender, args) => _dispatcherTimer.Stop();
        }

        public event EventHandler CloseRequest;

        public ServiceManager ServiceManager { get; }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ServiceManager.ActionManager.CheckTime();
        }

        public void Close()
        {
            
        }
    }
}
