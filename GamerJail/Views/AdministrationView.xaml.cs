using System;
using System.Windows;
using GamerJail.ViewModels.Pages;

namespace GamerJail.Views
{
    /// <summary>
    /// Interaktionslogik für AdministrationView.xaml
    /// </summary>
    public partial class AdministrationView
    {
        public AdministrationView()
        {
            InitializeComponent();
            Loaded += ConfigurationView_Loaded;
        }

        private void ConfigurationView_Loaded(object sender, RoutedEventArgs e)
        {
            ((AdministrationViewModel) DataContext).ClearPasswords += SettingsView_ClearPasswords;
        }

        private void SettingsView_ClearPasswords(object sender, EventArgs e)
        {
            PasswordBox1.Clear();
            PasswordBox2.Clear();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((AdministrationViewModel) DataContext).Password1Changed(PasswordBox1.SecurePassword);
        }

        private void PasswordBox2_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((AdministrationViewModel) DataContext).Password2Changed(PasswordBox2.SecurePassword);
        }
    }
}
