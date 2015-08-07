using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using GamerJail.Data;
using GamerJail.Logic;
using GamerJail.Shared;
using GamerJail.ViewManagement;
using PieControls;

namespace GamerJail.ViewModels.Pages
{
    class StatisticViewModel : PropertyChangedBase, IView
    {
        private readonly ServiceManager _serviceManager;
        private Statistics _currentStatistics;
        private TimePeriod _selectedPeriod;
        private ObservableCollection<PieSegment> _pieData;

        public StatisticViewModel(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            SelectedPeriod = TimePeriod.Day;
            CurrentStatistics = StatisticManager.GetStatistics(_serviceManager.DatabaseManager.PlayTimes,
                _serviceManager.DatabaseManager.Programs, SelectedPeriod);
        }

        public event EventHandler CloseRequest;

        public Statistics CurrentStatistics
        {
            get { return _currentStatistics; }
            set
            {
                if (SetProperty(value, ref _currentStatistics) && value != null)
                {
                    PieData = new ObservableCollection<PieSegment>(
                        value.Games.Select(
                            x =>
                                new PieSegment
                                {
                                    Name = x.Name,
                                    Value = x.TimePlayed.TotalMilliseconds,
                                    Color = x.ChartColor
                                }));
                }
            }
        }

        public ObservableCollection<PieSegment> PieData
        {
            get { return _pieData; }
            set { SetProperty(value, ref _pieData); }
        }

        public TimePeriod SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(value, ref _selectedPeriod))
                {
                    CurrentStatistics = StatisticManager.GetStatistics(_serviceManager.DatabaseManager.PlayTimes,
                        _serviceManager.DatabaseManager.Programs, value);
                }
            }
        }

        public void Close()
        {

        }
    }
}
