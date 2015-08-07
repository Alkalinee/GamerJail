using System;
using System.Linq;
using System.Windows.Threading;
using GamerJail.Shared;
using GamerJail.Utilities;

namespace GamerJail.Logic
{
    public class ActionManager : PropertyChangedBase
    {
        private readonly ServiceManager _serviceManager;
        private bool _isEnabled;
        private readonly DispatcherTimer _timer;
        private TimeSpan _timeLeft;
        private State _currentState = State.EverythingIsAwesome;

        public ActionManager(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(10), IsEnabled = false};
            _timer.Tick += _timer_Tick;
        }

        public event EventHandler StateChanged;
        public event EventHandler NothingIsAwesome;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    _timer.IsEnabled = value;
                    if (!value && ShutdownAction?.IsEnabled == true)
                        ShutdownAction.Cancel();
                }
            }
        }

        public TimeSpan TimeLeft
        {
            get { return _timeLeft; }
            set { SetProperty(value, ref _timeLeft); }
        }

        public State CurrentState
        {
            get { return _currentState; }
            set
            {
                if(SetProperty(value, ref _currentState))
                    StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ShutdownAction ShutdownAction { get; set; }

        public void CheckTime()
        {
            if (_serviceManager.IsFreeDay)
                return; //We don't even care

            if (ShutdownAction?.IsEnabled == true)
                return;

            var helper = new DateTimeHelper();
            _serviceManager.RefreshTimePlayedToday();

            switch (_serviceManager.Config.Mode)
            {
                case Mode.PerDay:
                    TimeLeft = _serviceManager.Config.GamingTimePerDay - _serviceManager.TimePlayedToday;
                    break;
                case Mode.PerWeek:
                    TimeLeft = TimeSpan.FromHours(_serviceManager.Config.GamingTimePerDay.TotalHours*7) -
                               TimeSpan.FromMilliseconds(
                                   _serviceManager.DatabaseManager.PlayTimes.Where(
                                       x => helper.IsCurrentWeek(x.Timestamp))
                                       .Select(x => x.Duration.TotalMilliseconds)
                                       .Sum()).Add(_serviceManager.Service.TimePlayed);
                    break;
                case Mode.PerDayWithSave:
                    var savedTime =
                        TimeSpan.FromHours(((int) DateTime.Now.DayOfWeek - 1)*
                                           _serviceManager.Config.GamingTimePerDay.TotalHours) -
                        TimeSpan.FromMilliseconds(
                            _serviceManager.DatabaseManager.PlayTimes.Where(x => helper.IsCurrentWeek(x.Timestamp))
                                .Select(x => x.Duration.TotalMilliseconds)
                                .Sum());
                    if (savedTime > TimeSpan.Zero)
                    {
                        TimeLeft = (_serviceManager.Config.GamingTimePerDay + savedTime) -
                                   _serviceManager.TimePlayedToday;
                    }
                    else
                    {
                        TimeLeft = _serviceManager.Config.GamingTimePerDay - _serviceManager.TimePlayedToday;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (DateTime.Now < DateTime.Today.Date.AddHours(_serviceManager.Config.TimeSpan.FromTime) ||
                DateTime.Now > DateTimeHelper.GetToday().AddHours(_serviceManager.Config.TimeSpan.ToTime))
            {
                TimeLeft = TimeSpan.Zero;
            }
            else
            {
                var time = DateTimeHelper.GetToday().AddMinutes(_serviceManager.Config.TimeSpan.ToTime*30) -
                           DateTime.Now;
                if (TimeLeft < time)
                    TimeLeft = time;
            }

            if (_serviceManager.Service.CurrentProgram == null) //No actions when no game is running
                return;

            if (TimeLeft < TimeSpan.FromMinutes(30) && TimeLeft > TimeSpan.FromMinutes(5))
            {
                CurrentState = State.ComingToEnd;
            }
            else if (TimeLeft < TimeSpan.FromMinutes(5) && TimeLeft > TimeSpan.Zero)
            {
                CurrentState = State.Critical;
            }
            else if (TimeLeft < TimeSpan.Zero || TimeLeft == TimeSpan.Zero)
            {
                CurrentState = State.StopDaShitImmediately;

                ShutdownAction = new ShutdownAction();
                ShutdownAction.Start();
                ShutdownAction.InvokeActions += (sender, args) => NothingIsAwesome?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                CurrentState = State.EverythingIsAwesome;
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            CheckTime();
        }
    }

    public enum State
    {
        EverythingIsAwesome,
        ComingToEnd,
        Critical,
        StopDaShitImmediately
    }
}
