using System;
using System.Diagnostics;
using Jugenschutzprogramm.Shared;

namespace Jugenschutzprogramm_Installer.ViewManagement.Views
{
    class Step2 : View
    {
        private int _lowerValue;
        private int _upperValue = 48;
        private bool _showWarning;

        public Step2(Config config) : base(config)
        {
            Config.TimeSpan.FromTime = 0;
            Config.TimeSpan.ToTime = 24;
        }

        public int LowerValue
        {
            get { return _lowerValue; }
            set
            {
                if (_lowerValue != value)
                {
                    _lowerValue = value;
                    var timeSpan = TimeSpan.FromMinutes(value * 30 + 4 * 60);
                    Config.TimeSpan.FromTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    Debug.Print("FromTime: " + Config.TimeSpan.FromTime);
                    CheckIfEverythingIsAwesome();
                }
            }
        }

        public int UpperValue
        {
            get { return _upperValue; }
            set
            {
                if (_upperValue != value)
                {
                    _upperValue = value;
                    var timeSpan = TimeSpan.FromMinutes(value * 30 + 4 * 60);
                    Config.TimeSpan.ToTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    Debug.Print("ToTime: " + Config.TimeSpan.ToTime);
                    CheckIfEverythingIsAwesome();
                }
            }
        }

        public bool ShowWarning
        {
            get { return _showWarning; }
            set { SetProperty(value, ref _showWarning); }
        }

        private void CheckIfEverythingIsAwesome()
        {
            ShowWarning = (UpperValue*30 - LowerValue*30) < Config.GamingTimePerDay.TotalMinutes;
        }
    }
}
