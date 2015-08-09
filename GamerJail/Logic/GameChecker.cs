using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using GamerJail.Native;

namespace GamerJail.Logic
{
    class GameChecker
    {
        private static readonly string[] SomeGames =
        {
            "RocketLeague", "League of Legends", "dota2", "Minecraft",
            "SamHD_TSE", "csgo", "war3"
        };

        public static bool IsGame(Process process)
        {
            process.Refresh();
            Debug.Print("Analyisere Spiel: " + process.ProcessName);

            if (SomeGames.Contains(process.ProcessName))
                return true;

            var cname = GetClassName(process.MainWindowHandle);
            if (cname == "Progman" || cname == "WorkerW") //This are Windows processes
                return false;

            if (cname == "LaunchUnrealUWindowsClient")
                return true;

            var flag1 = process.PrivateMemorySize64 > 471859200; //Check if RAM > 450 MiB
            var flag2 = WindowIsFullscreen(process.MainWindowHandle);
            if (!flag1)
                Debug.Print("No memory");
            if (!flag2)
                Debug.Print("No fullscreen");

            if (flag1 & flag2)
                return true;

            return false;
        }

        private static bool WindowIsFullscreen(IntPtr window)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            UnsafeNativeMethods.GetWindowPlacement(window, ref placement);
            var workarea = SystemParameters.WorkArea;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return placement.normalPosition.left == 0 && placement.normalPosition.top == 0 &&
                   placement.normalPosition.Width >= workarea.Width &&
                   placement.normalPosition.Height > workarea.Height;
        }

        public static string GetClassName(IntPtr handle)
        {
            const int maxChars = 256;
            var className = new StringBuilder(maxChars);
            if (UnsafeNativeMethods.GetClassName(handle, className, maxChars) > 0)
            {
                return className.ToString();
            }
            return string.Empty;
        }
    }
}