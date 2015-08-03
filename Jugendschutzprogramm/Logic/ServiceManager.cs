using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Jugendschutzprogramm.Data;
using Jugendschutzprogramm.Native;
using Jugendschutzprogramm.Utilities;
using Jugenschutzprogramm.Shared;

namespace Jugendschutzprogramm.Logic
{
    class ServiceManager : PropertyChangedBase
    {
        private TimeSpan _timePlayedToday;
        private bool _isFreeDay;
        private Service _service;
        private Config _config;
        private static ServiceManager _instance;

        private ServiceManager()
        {
        }

        public void Load()
        {
            var configFile = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jugendschutzprogramm", "config.xml"));
            _config = new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText(configFile.FullName));
            GC.Collect(); //To clear the string

            DatabaseManager.Password = Encoding.BigEndianUnicode.GetString(MD5.Create("zt+6Mr5b}j?Xe^chJ{Y8+vDn+?6jCqr37]%SRE~__@\" * "));
            DatabaseManager = new DatabaseManager();
            DatabaseManager.Load();

            _service = new Service(this);
            _service.Enable();
        }

        public static ServiceManager Current => _instance ?? (_instance = new ServiceManager());

        public DatabaseManager DatabaseManager { get; set; }

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
                        .Sum(x => x.Duration.TotalMilliseconds)).Add(_service.TimePlayed);
            Debug.Print("refresh");
        }
    }
}