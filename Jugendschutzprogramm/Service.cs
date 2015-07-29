using System;
using Jugendschutzprogramm.Native;

namespace Jugendschutzprogramm
{
    class Service
    {
        // ReSharper disable InconsistentNaming
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        // ReSharper restore InconsistentNaming

        private Service _instance;
        private readonly UnsafeNativeMethods.WinEventDelegate _delegate;
        private IntPtr _hook;

        private Service()
        {
            _delegate = WinEventProc;
        }

        public Service Current => _instance ?? (_instance = new Service());
        public bool IsEnabled { get; set; }

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

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            
        }
    }
}
