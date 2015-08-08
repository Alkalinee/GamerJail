using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using GamerJail.Utilities;

namespace GamerJail.Data
{
    class StatisticManager
    {
        // ReSharper disable PossibleNullReferenceException
        private static readonly Color[] Colors =
        {
            (Color) ColorConverter.ConvertFromString("#1abc9c"), (Color) ColorConverter.ConvertFromString("#3498db"),
            (Color) ColorConverter.ConvertFromString("#9b59b6"), (Color) ColorConverter.ConvertFromString("#34495e"),
            (Color) ColorConverter.ConvertFromString("#e74c3c"), (Color) ColorConverter.ConvertFromString("#e67e22"),
            (Color) ColorConverter.ConvertFromString("#f1c40f"), (Color) ColorConverter.ConvertFromString("#16a085"),
            (Color) ColorConverter.ConvertFromString("#27ae60"), (Color) ColorConverter.ConvertFromString("#d35400"),
            (Color) ColorConverter.ConvertFromString("#bdc3c7")
        };
        // ReSharper restore PossibleNullReferenceException
        private static readonly Random Random = new Random();
        private static readonly Dictionary<Program, Color> CachedColors = new Dictionary<Program, Color>();
        private static int _colorCounter = 0;

        public static Statistics GetStatistics(IList<PlayTime> playTimes, IList<Program> programs, TimePeriod timePeriod)
        {
            var helper = new DateTimeHelper();
            List<PlayTime> times;
            switch (timePeriod)
            {
                case TimePeriod.Day:
                    times = playTimes.Where(x => helper.IsToday(x.Timestamp)).ToList();
                    break;
                case TimePeriod.Week:
                    times = playTimes.Where(x => helper.IsCurrentWeek(x.Timestamp)).ToList();
                    break;
                case TimePeriod.Month:
                    times = playTimes.Where(x => helper.IsCurrentMonth(x.Timestamp)).ToList();
                    break;
                case TimePeriod.Year:
                    times = playTimes.Where(x => helper.IsCurrentYear(x.Timestamp)).ToList();
                    break;
                case TimePeriod.Ever:
                    times = playTimes.ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timePeriod), timePeriod, null);
            }

            var statistic = new Statistics
            {
                TimePeriod = timePeriod,
                TimePlayed = TimeSpan.FromMilliseconds(times.Select(x => x.Duration.TotalMilliseconds).Sum())
            };

            var games = new List<GameStatistic>();
            foreach (var gameId in times.Select(x => x.Program).Distinct())
            {
                var game = programs.FirstOrDefault(x => x.Guid == gameId);
                if (game == null)
                    continue;

                games.Add(new GameStatistic
                {
                    Name = game.Name,
                    Icon = game.Icon,
                    Guid = game.Guid,
                    ChartColor = GetColor(game),
                    TimePlayed =
                        TimeSpan.FromMilliseconds(
                            times.Where(x => x.Program == game.Guid).Select(x => x.Duration.TotalMilliseconds).Sum())
                });
            }

            statistic.Games = games.OrderByDescending(x => x.TimePlayed).ToList();
            return statistic;
        }

        private static Color GetColor(Program program)
        {
            if (CachedColors.ContainsKey(program))
                return CachedColors[program];

            var color = _colorCounter > Colors.Length ? PickBrush() : Colors[_colorCounter];
            _colorCounter++;
            CachedColors.Add(program, color);
            return color;
        }

        private static Color PickBrush()
        {
            Type brushesType = typeof(Colors);
            PropertyInfo[] properties = brushesType.GetProperties();

            int random = Random.Next(properties.Length);
            return (Color)properties[random].GetValue(null, null);
        }
    }
}
