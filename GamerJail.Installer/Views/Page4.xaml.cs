using System.Windows;
using System.Windows.Controls;
using GamerJail.Installer.ViewManagement.Views;

namespace GamerJail.Installer.Views
{
    /// <summary>
    /// Interaktionslogik für Page4.xaml
    /// </summary>
    public partial class Page4
    {
        public Page4()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((Step3)DataContext).Password1Changed(((PasswordBox)sender).SecurePassword);
        }

        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((Step3)DataContext).Password2Changed(((PasswordBox)sender).SecurePassword);
        }
    }
}
