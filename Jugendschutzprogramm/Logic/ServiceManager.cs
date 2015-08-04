using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Web.Script.Serialization;
using Hardcodet.Wpf.TaskbarNotification;
using Jugendschutzprogramm.Data;
using Jugendschutzprogramm.Native;
using Jugendschutzprogramm.Utilities;
using Jugenschutzprogramm.Shared;

namespace Jugendschutzprogramm.Logic
{
    class ServiceManager : PropertyChangedBase
    {
        private TaskbarIcon _taskbarIcon;
        private TimeSpan _timePlayedToday;
        private bool _isFreeDay;
        private static ServiceManager _instance;

        private ServiceManager()
        {
        }

        public void Load(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;

            var configFile = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jugendschutzprogramm", "config.xml"));
            Config = new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText(configFile.FullName));
            GC.Collect(); //To clear the string

            DatabaseManager.Password = Encoding.BigEndianUnicode.GetString(MD5.Create("zt+6Mr5b}j?Xe^chJ{Y8+vDn+?6jCqr37]%SRE~__@\" * "));
            DatabaseManager = new DatabaseManager();
            DatabaseManager.Load();

            Service = new Service(this);
            Service.Enable();

            ActionManager = new ActionManager(this);
            ActionManager.CheckTime();
            ActionManager.StateChanged += ActionManager_StateChanged;
            ActionManager.NothingIsAwesome += ActionManager_NothingIsAwesome;

            Service.Started += (sender, args) => ActionManager.IsEnabled = true;
            Service.Stopped += (sender, args) => ActionManager.IsEnabled = false;
        }

        public static ServiceManager Current => _instance ?? (_instance = new ServiceManager());

        public DatabaseManager DatabaseManager { get; private set; }
        public Config Config { get; private set; }
        public Service Service { get; private set; }
        public ActionManager ActionManager { get; private set; }

        public TimeSpan TimePlayedToday
        {
            get { return _timePlayedToday; }
            set { SetProperty(value, ref _timePlayedToday); }
        }

        public bool IsFreeDay
        {
            get { return _isFreeDay; }
            set { SetProperty(value, ref _isFreeDay); }
        }

        public void RefreshTimePlayedToday()
        {
            var dateTimeHelper = new DateTimeHelper();
            TimePlayedToday =
                TimeSpan.FromMilliseconds(
                    DatabaseManager.PlayTimes.Where(x => dateTimeHelper.IsToday(x.Timestamp))
                        .Sum(x => x.Duration.TotalMilliseconds)).Add(Service.TimePlayed);
            Debug.Print("refresh");
        }

        private void ActionManager_StateChanged(object sender, EventArgs e)
        {
            var soundPlayer = new SoundPlayer();
            switch (ActionManager.CurrentState)
            {
                case State.EverythingIsAwesome:
                    return;
                case State.ComingToEnd:
                    _taskbarIcon.ShowBalloonTip("Jugendschutzprogramm", "In 30 Minuten ist ihre Spielzeit zu Ende.", BalloonIcon.Info);
                    soundPlayer.Stream = Resources.Properties.Resources.Announcer1;
                    break;
                case State.Critical:
                    _taskbarIcon.ShowBalloonTip("Jugendschutzprogramm", "In 5 Minuten ist ihre Spielzeit abgelaufen.",
                        BalloonIcon.Warning);
                    soundPlayer.Stream = Resources.Properties.Resources.Announcer2;
                    break;
                case State.StopDaShitImmediately:
                    _taskbarIcon.ShowBalloonTip("Jugendschutzprogramm",
                        Config.Actions == Actions.Nothing
                            ? "Ihre Spielzeit ist nun vorbei. Bitte beenden Sie das Spiel!"
                            : "Ihre Spielzeit ist nun vorbei. Bitte beenden Sie das Spiel oder wir werden Maßnahmen ergreifen!",
                        BalloonIcon.Warning);

                    soundPlayer.Stream = Config.Actions != Actions.Nothing
                        ? Resources.Properties.Resources.Announcer3_1
                        : Resources.Properties.Resources.Announcer3_2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            soundPlayer.Play();
        }

        private void ActionManager_NothingIsAwesome(object sender, EventArgs e)
        {
            switch (Config.Actions)
            {
                case Actions.Nothing:
                    return;
                case Actions.CloseGame:
                    Service.CloseCurrentProcess();
                    break;
                case Actions.ShutdownComputer:
                    Process.Start("shutdown", "/s now");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}