using System;
using System.Diagnostics;
using GamerJail.Installer.Model;

namespace GamerJail.Installer.ViewManagement.Views
{
    class Step2 : View
    {
        private int _lowerValue;
        private int _upperValue = 48;
        private bool _showWarning;

        public Step2(Setup setup) : base(setup)
        {
            setup.Config.TimeSpan.FromTime = 0;
            setup.Config.TimeSpan.ToTime = 24;
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
                    Setup.Config.TimeSpan.FromTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    Debug.Print("FromTime: " + Setup.Config.TimeSpan.FromTime);
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
                    Setup.Config.TimeSpan.ToTime = timeSpan.TotalHours - (timeSpan.TotalHours > 24 ? 24 : 0);
                    Debug.Print("ToTime: " + Setup.Config.TimeSpan.ToTime);
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
            ShowWarning = (UpperValue*30 - LowerValue*30) < Setup.Config.GamingTimePerDay.TotalMinutes;
        }
    }
}
