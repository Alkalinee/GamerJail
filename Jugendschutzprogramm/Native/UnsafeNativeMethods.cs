using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jugendschutzprogramm.Native
{
   static class UnsafeNativeMethods
   {
       internal delegate void WinEventDelegate(
           IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
           uint dwmsEventTime);

       [DllImport("user32.dll")]
       internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
           WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

       [DllImport("user32.dll")]
       internal static extern IntPtr GetForegroundWindow();

       [DllImport("user32.dll")]
       internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

       [DllImport("user32.dll")]
       internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);
   }
}
