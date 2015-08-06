using System;
using System.Windows;
using GamerJail.Logic;
using GamerJail.Utilities;
using GamerJail.ViewModels;

namespace GamerJail.Views
{
    /// <summary>
    /// Interaktionslogik für ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow
    {
        public ConfigurationWindow(ServiceManager serviceManager)
        {
            DataContext = new ConfigurationViewModel(serviceManager, this);
            ((ConfigurationViewModel)DataContext).ClearPasswords += SettingsView_ClearPasswords;
            InitializeComponent();
            SourceInitialized += ConfigurationWindow_SourceInitialized;
        }

        private void ConfigurationWindow_SourceInitialized(object sender, EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void SettingsView_ClearPasswords(object sender, EventArgs e)
        {
            PasswordBox1.Clear();
            PasswordBox2.Clear();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ConfigurationViewModel) DataContext).Password1Changed(PasswordBox1.SecurePassword);
        }

        private void PasswordBox2_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ConfigurationViewModel)DataContext).Password2Changed(PasswordBox2.SecurePassword);
        }
    }
}
