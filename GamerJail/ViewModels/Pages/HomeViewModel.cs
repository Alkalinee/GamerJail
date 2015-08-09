using System;
// ReSharper disable RedundantUsingDirective
using System.ComponentModel;
// ReSharper restore RedundantUsingDirective
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Utilities;
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

            NewPlayTime = ServiceManager.Config.Mode == Mode.PerWeek
                ? new DateTimeHelper().NextWeek
                : new DateTimeHelper().NextDay;

            PlayTimeSpan = ServiceManager.Config.TimeSpan.FromTime == 4 && ServiceManager.Config.TimeSpan.ToTime == 28
                ? "Immer"
                : $"{TimeSpan.FromHours(ServiceManager.Config.TimeSpan.FromTime > 24 ? ServiceManager.Config.TimeSpan.FromTime - 24 : ServiceManager.Config.TimeSpan.FromTime).ToString("hh\\:mm")} bis {TimeSpan.FromHours(ServiceManager.Config.TimeSpan.ToTime > 24 ? ServiceManager.Config.TimeSpan.ToTime - 24 : ServiceManager.Config.TimeSpan.ToTime).ToString("hh\\:mm")}";
        }

        public event EventHandler CloseRequest;

        public ServiceManager ServiceManager { get; }
        public string PlayTimeSpan { get; }
        public DateTime NewPlayTime { get; }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ServiceManager.ActionManager.CheckTime();
        }

        public void Close()
        {
            
        }
    }
}
