using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Win32;

namespace Jugendschutzprogramm.Data
{
    class DatabaseManager
    {
        public static string Password;

        private readonly string _databasePath;
        private SQLiteConnection _connection;

        public DatabaseManager()
        {
            _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Jugendschutzprogramm", "database.sqlite");
        }

        public ObservableCollection<Program> Programs { get; set; }
        public ObservableCollection<PlayTime> PlayTimes { get; set; }

        public void Load()
        {
            var databaseFile = new FileInfo(_databasePath);
            if (!databaseFile.Exists)
            {
                SQLiteConnection.CreateFile(databaseFile.FullName);
                _connection = new SQLiteConnection($"Data Source={databaseFile.FullName};Version=3;");
                _connection.Open();
                //_connection.ChangePassword(Password);

                using (
                    var command =
                        new SQLiteCommand(
                            "CREATE TABLE `PlayTimes` (Timestamp DATETIME NOT NULL, Duration VARCHAR(50), Program VARCHAR(36), Guid VARCHAR(36) NOT NULL, PRIMARY KEY (Guid))",
                            _connection))
                    command.ExecuteNonQuery();

                using (
                    var command =
                        new SQLiteCommand(
                            "CREATE TABLE `Programs` (Timestamp DATETIME NOT NULL, Path VARCHAR(50), IsGame INT, Name VARCHAR(512), Guid VARCHAR(36) NOT NULL, PRIMARY KEY (Guid))",
                            _connection))
                    command.ExecuteNonQuery();
            }
            else
            {
                _connection = new SQLiteConnection($"Data Source={databaseFile.FullName};Version=3;"); //Password={Password};
                _connection.Open();
            }

            PlayTimes = new ObservableCollection<PlayTime>();
            using (var command = new SQLiteCommand("SELECT * FROM `PlayTimes`", _connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PlayTimes.Add(new PlayTime
                    {
                        Timestamp = reader.GetDateTime(0),
                        Duration = XmlConvert.ToTimeSpan(reader[1].ToString()),
                        Program = Guid.ParseExact(reader[2].ToString(), "D"),
                        Guid = Guid.ParseExact(reader[3].ToString(), "D")
                    });
                }
            }

            Programs = new ObservableCollection<Program>();
            using (var command = new SQLiteCommand("SELECT * FROM `Programs`", _connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Programs.Add(new Program
                    {
                        Timestamp = reader.GetDateTime(0),
                        Path = reader[1].ToString(),
                        IsGame = reader.GetInt32(2) == 1,
                        Name = reader[3]?.ToString(),
                        Guid = Guid.ParseExact(reader[4].ToString(), "D")
                    });
                }
            }

            var time = PlayTimes.OrderByDescending(x => x.Timestamp).First();
            if (time.Duration == TimeSpan.Zero)
                PlayTimeFinished(time, GetLastSystemShutdown());
        }

        public PlayTime StartPlayTime(Program program)
        {
            var playTime = new PlayTime
            {
                Guid = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Program = program.Guid,
                Duration = TimeSpan.Zero
            };
            PlayTimes.Add(playTime);

            using (
                var command =
                    new SQLiteCommand(
                        "INSERT INTO `PlayTimes` (Timestamp, Duration, Program, Guid) VALUES (@timestamp, @duration, @program, @guid)",
                        _connection))
            {
                command.Parameters.AddWithValue("@timestamp", playTime.Timestamp);
                command.Parameters.AddWithValue("@duration", XmlConvert.ToString(playTime.Duration));
                command.Parameters.AddWithValue("@program", playTime.Program.ToString("D"));
                command.Parameters.AddWithValue("@guid", playTime.Guid.ToString("D"));
                command.ExecuteNonQuery();
            }

            return playTime;
        }

        public void PlayTimeFinished(PlayTime playTime, DateTime time)
        {
            var duration = TimeSpan.FromMilliseconds((time - playTime.Timestamp).TotalMilliseconds);
            playTime.Duration = duration;
            using (
                var command =
                    new SQLiteCommand(
                        $"UPDATE `PlayTimes` SET Duration=@duration WHERE Guid='{playTime.Guid.ToString("D")}'",
                        _connection))
            {
                command.Parameters.AddWithValue("@duration", XmlConvert.ToString(duration));
                command.ExecuteNonQuery();
            }
        }

        public Program AddProgram(string path, bool isGame, string name)
        {
            var program = new Program
            {
                Path = path,
                Guid = Guid.NewGuid(),
                Name = name,
                IsGame = isGame,
                Timestamp = DateTime.Now
            };

            using (
                var command =
                    new SQLiteCommand(
                        "INSERT INTO `Programs` (Timestamp, Path, IsGame, Name, Guid) VALUES (@timestamp, @path, @isgame, @name, @guid)",
                        _connection))
            {
                command.Parameters.AddWithValue("@timestamp", program.Timestamp);
                command.Parameters.AddWithValue("@path", program.Path);
                command.Parameters.AddWithValue("@isgame", isGame ? 1 : 0);
                command.Parameters.AddWithValue("@name", program.Name);
                command.Parameters.AddWithValue("@guid", program.Guid.ToString("D"));
                command.ExecuteNonQuery();
            }

            Programs.Add(program);

            return program;
        }

        private static DateTime GetLastSystemShutdown()
        {
            string sKey = @"System\CurrentControlSet\Control\Windows";
            var key = Registry.LocalMachine.OpenSubKey(sKey);

            string sValueName = "ShutdownTime";
            byte[] val = (byte[]) key.GetValue(sValueName);
            long valueAsLong = BitConverter.ToInt64(val, 0);
            return DateTime.FromFileTime(valueAsLong);
        }

        ~DatabaseManager()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
