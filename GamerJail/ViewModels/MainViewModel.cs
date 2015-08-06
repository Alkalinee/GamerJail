using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Views;

namespace GamerJail.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private RelayCommand _openConfigurationCommand;

        public MainViewModel()
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            ServiceManager = ServiceManager.Current;
            _dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
            Application.Current.Windows.Cast<Window>().First().Closed += (sender, args) => _dispatcherTimer.Stop();
        }

        public ServiceManager ServiceManager { get; }

        public RelayCommand OpenConfigurationCommand
        {
            get
            {
                return _openConfigurationCommand ?? (_openConfigurationCommand = new RelayCommand(parameter =>
                {
                    var passwordWindow = new PasswordDialogWindow {Owner = Application.Current.MainWindow};
                    if (passwordWindow.ShowDialog() != true)
                        return;

                    var window = new ConfigurationWindow(ServiceManager) {Owner = Application.Current.MainWindow};
                    window.ShowDialog();
                }));
            }
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ServiceManager.ActionManager.CheckTime();
        }
    }
}