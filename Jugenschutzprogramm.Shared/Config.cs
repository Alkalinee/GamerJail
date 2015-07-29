using System;
using System.Collections.Generic;
using System.Security;
using Jugenschutzprogramm.Shared.Utilities;

namespace Jugenschutzprogramm.Shared
{
    public class Config : PropertyChangedBase
    {
        private Mode _mode;
        private TimeSpan _gamingTimePerDay = System.TimeSpan.FromHours(3.5);
        private Actions _actions;
        private DayTimeSpan _timeSpan;
        private bool _useTcpConnection;
        private int _tcpPort;
        private ProtectionLevel _protectionLevel = ProtectionLevel.Default;

        public Config()
        {
            TimeSpan = new DayTimeSpan();
            ProcessWhitelist = new List<string>();
        }

        public Mode Mode
        {
            get { return _mode; }
            set { SetProperty(value, ref _mode); }
        }

        public TimeSpan GamingTimePerDay
        {
            get { return _gamingTimePerDay; }
            set { SetProperty(value, ref _gamingTimePerDay); }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set { SetProperty(value, ref _protectionLevel); }
        }

        /// <summary>
        /// What to do if the user doesn't react
        /// </summary>
        public Actions Actions
        {
            get { return _actions; }
            set { SetProperty(value, ref _actions); }
        }

        /// <summary>
        /// The time span where the user can play games
        /// </summary>
        public DayTimeSpan TimeSpan
        {
            get { return _timeSpan; }
            private set { SetProperty(value, ref _timeSpan); }
        }

        public bool UseTcpConnection
        {
            get { return _useTcpConnection; }
            set { SetProperty(value, ref _useTcpConnection); }
        }

        public int TcpPort
        {
            get { return _tcpPort; }
            set { SetProperty(value, ref _tcpPort); }
        }

        public SecureString Password { get; set; }

        public string PasswordSerialize
        {
            get { return Password.SecureStringToString(); }
            set
            {
                Password = value.ToSecureString();
            }
        }

        public List<string> ProcessWhitelist { get; set; }
    }

    public enum Actions
    {
        Nothing,
        CloseGame,
        ShutdownComputer
    }

    public enum Mode
    {
        /// <summary>
        /// The user gets every day some hours
        /// </summary>
        PerDay,
        /// <summary>
        /// The user gets every week some hours
        /// </summary>
        PerWeek,
        /// <summary>
        /// The user gets every day some hours. If he doesn't use all of them, he can use them the next day (gets reseted every week)
        /// </summary>
        PerDayWithSave
    }

    public enum ProtectionLevel
    {
        Nothing,
        Default,
        Advanced
    }
}