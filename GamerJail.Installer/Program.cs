using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;

namespace GamerJail.Installer
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            App.Main();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            var path  = $"pack://application:,,,/GamerJail.Installer;component/InstallationFiles/{assemblyName.Name}.dll";
            StreamResourceInfo resource;
            try
            {
                resource = Application.GetResourceStream(new Uri(path));
            }
            catch (IOException)
            {
                return null;
            }

            if (resource == null)
                return null;

            using (var stream = resource.Stream)
            {
                byte[] assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
