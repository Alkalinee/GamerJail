using System;
using System.Diagnostics;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// Represents the background theme of the application.
    /// </summary>
    [DebuggerDisplay("apptheme={Name}, theme={Theme}, res={Resources.Source}")]
    public class AppTheme
    {
        /// <summary>
        /// The ResourceDictionary that represents this application theme.
        /// </summary>
        public ResourceDictionary Resources {get; }

        /// <summary>
        /// Gets the name of the application theme.
        /// </summary>
        public string Name { get; }

        public AppTheme(string name, Uri resourceAddress)
        {
            if(name == null)
                throw new ArgumentException("name");

            if(resourceAddress == null)
                throw new ArgumentNullException("resourceAddress");

            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}