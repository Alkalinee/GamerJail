using System;
using System.Collections.Generic;
using System.Linq;
using GamerJail.Data;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.Utilities;
using GamerJail.ViewManagement;

namespace GamerJail.ViewModels.Pages
{
    class HistoryViewModel : PropertyChangedBase, IView
    {
        private readonly ServiceManager _serviceManager;
        private int _year;
        private List<HistoryEntry> _historyEntries;

        public HistoryViewModel(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            Years = serviceManager.DatabaseManager.PlayTimes.Select(x => x.Timestamp.Year).Distinct().ToList();
            Year = Years.LastOrDefault();
        }

        public event EventHandler CloseRequest;

        public int Year
        {
            get { return _year; }
            set
            {
                if (SetProperty(value, ref _year) && value != 0)
                {
                    HistoryEntries =
                        _serviceManager.DatabaseManager.PlayTimes.Where(x => x.Timestamp.Year == value)
                            .GroupBy(x => DateTimeHelper.GetDateTime(x.Timestamp))
                            .Select(x => new HistoryEntry {DateTime = x.Key, Games = GetGames(x.ToList())})
                            .Reverse()
                            .ToList();
                }
            }
        }

        public List<HistoryEntry> HistoryEntries
        {
            get { return _historyEntries; }
            set { SetProperty(value, ref _historyEntries); }
        }

        public List<int> Years { get; set; }

        public void Close()
        {
            
        }

        private List<GameStatistic> GetGames(List<PlayTime> playTimes)
        {
            return
                playTimes.Select(x => _serviceManager.DatabaseManager.Programs.First(y => y.Guid == x.Program))
                    .Distinct()
                    .Select(
                        x =>
                            new GameStatistic
                            {
                                Guid = x.Guid,
                                Name = x.Name,
                                Icon = x.Icon,
                                TimePlayed =
                                    TimeSpan.FromMilliseconds(
                                        playTimes.Where(y => y.Program == x.Guid)
                                            .Select(y => y.Duration.TotalMilliseconds)
                                            .Sum())
                            }).ToList();
        }
    }
}
