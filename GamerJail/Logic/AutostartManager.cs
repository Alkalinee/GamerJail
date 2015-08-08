using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace GamerJail.Logic
{
   static class AutostartManager
    {
        public static void TryAdd()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
                return;

            if (key.GetSubKeyNames().Contains("GamerJail"))
                key.SetValue("GamerJail", "\"" + Assembly.GetExecutingAssembly().Location + "\"");
        }

        public static void Remove()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
                return;

            if (key.GetSubKeyNames().Contains("GamerJail"))
                key.DeleteSubKey("GamerJail");
        }
    }
}
