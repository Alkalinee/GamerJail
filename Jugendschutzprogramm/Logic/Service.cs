using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Jugendschutzprogramm.Data;
using Jugendschutzprogramm.Native;
using Jugendschutzprogramm.Utilities;

namespace Jugendschutzprogramm.Logic
{
    class Service : PropertyChangedBase
    {
        private readonly ServiceManager _manager;
        // ReSharper disable InconsistentNaming
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        // ReSharper restore InconsistentNaming

        private readonly UnsafeNativeMethods.WinEventDelegate _delegate;
        private IntPtr _hook;
        private bool _isInGame;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _timeGamingStarted;
        private Process _currentProcess;
        private Program _currentProgram;

        public Service(ServiceManager manager)
        {
            _manager = manager;
            _delegate = WinEventProc;
        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        public bool IsEnabled { get; set; }

        public Program CurrentProgram
        {
            get { return _currentProgram; }
            set { SetProperty(value, ref _currentProgram); }
        }

        public TimeSpan TimePlayed
        {
            get
            {
                if(!_isInGame)
                    return TimeSpan.Zero;

                return TimeSpan.FromMilliseconds((DateTime.Now - _timeGamingStarted).TotalMilliseconds);
            }
        }

        public void Enable()
        {
            if (IsEnabled)
                return;

            _hook = UnsafeNativeMethods.SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _delegate, 0, 0, WINEVENT_OUTOFCONTEXT);
            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled)
                return;

            UnsafeNativeMethods.UnhookWinEvent(_hook);
            IsEnabled = false;
        }

        public void CloseCurrentProcess()
        {
            _currentProcess?.Kill();
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            uint pid;
            UnsafeNativeMethods.GetWindowThreadProcessId(hwnd, out pid);
            var p = Process.GetProcessById((int) pid);
            NewProcess(p);
        }

        private async void NewProcess(Process process)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            string path;
            try
            {
                path = process.GetExecutablePath();
            }
            catch (Exception)
            {
                return;
            }

            foreach (var program in _manager.DatabaseManager.Programs)
            {
                if (string.Equals(program.Path, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (program.IsGame)
                        GameTimeStart(process, program);
                    return;
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(20), _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            var isGame = GameChecker.IsGame(process);
            var newProgram = _manager.DatabaseManager.AddProgram(path, isGame, process.ProcessName);

            if (isGame)
            {
                Debug.Print($"New Game found: {process.ProcessName} :: \"{path}\"");
                GameTimeStart(process, newProgram);
            }
        }

        private async void GameTimeStart(Process process, Program program)
        {
            if (_isInGame)
                return;

            CurrentProgram = program;
            _isInGame = true;
            _currentProcess = process;
            var playTime = _manager.DatabaseManager.StartPlayTime(program);
            _timeGamingStarted = DateTime.Now;
            Started?.Invoke(this, EventArgs.Empty);
            await Task.Run(() => process.WaitForExit());
            _currentProcess = null;
            _manager.DatabaseManager.PlayTimeFinished(playTime, DateTime.Now);
            _isInGame = false;
            Stopped?.Invoke(this, EventArgs.Empty);
            CurrentProgram = null;
        }
    }
}