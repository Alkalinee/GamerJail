using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Jugendschutzprogramm.Logic;

namespace Jugendschutzprogramm.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private readonly DispatcherTimer _dispatcherTimer;
        public MainViewModel()
        {
            ServiceManager = ServiceManager.Current;
            _dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
            Application.Current.Windows.Cast<Window>().First().Closed += (sender, args) => _dispatcherTimer.Stop();
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ServiceManager.RefreshTimePlayedToday();
        }

        public ServiceManager ServiceManager { get; }
    }
}