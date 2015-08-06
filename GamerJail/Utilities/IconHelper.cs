using System;
using System.Windows;
using System.Windows.Interop;
using GamerJail.Native;

namespace GamerJail.Utilities
{
    internal static class IconHelper
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const uint WM_SETICON = 0x0080;

        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            // Change the extended window style to not show a window icon
            int extendedStyle = UnsafeNativeMethods.GetWindowLong(hwnd, GWL_EXSTYLE);
            UnsafeNativeMethods.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);
            // Update the window's non-client area to reflect the changes
            UnsafeNativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE |
                                                                            SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
}
