using System;
using System.Diagnostics;
using System.Text;
using Jugendschutzprogramm.Native;

namespace Jugendschutzprogramm.Utilities
{
    static class ProcessExtensions
    {
        public static string GetExecutablePath(this Process process)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = UnsafeNativeMethods.OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, false, process.Id);
            if (hprocess == IntPtr.Zero)
                return string.Empty;

            try
            {
                int size = buffer.Capacity;
                if (UnsafeNativeMethods.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                {
                    return buffer.ToString();
                }
            }
            finally
            {
                UnsafeNativeMethods.CloseHandle(hprocess);
            }
            return string.Empty;
        }
    }
}