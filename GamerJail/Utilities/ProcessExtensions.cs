using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using GamerJail.Native;

namespace GamerJail.Utilities
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

        public static ImageSource GetFileImage(this string path)
        {
            if (!File.Exists(path))
                return null;
           return Icon.ExtractAssociatedIcon(path).ToImageSource();
        }
    }
}