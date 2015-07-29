using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows;
using Jugenschutzprogramm.Shared;

namespace Jugendschutzprogramm
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var configFile = new FileInfo("");
            var config = new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText(configFile.FullName));
            GC.Collect(); //To clear the string
            new Service(config);
        }
    }
}