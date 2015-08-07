using System;
using GamerJail.Utilities;
using GamerJail.ViewModels;

namespace GamerJail.Views
{
    /// <summary>
    /// Interaktionslogik für PasswordDialogWindow.xaml
    /// </summary>
    public partial class PasswordDialogWindow 
    {
        public PasswordDialogWindow()
        {
            DataContext = new PasswordDialogViewModel(this);
            InitializeComponent();
            SourceInitialized += PasswordDialogWindow_SourceInitialized;
        }

        private void PasswordDialogWindow_SourceInitialized(object sender, EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
    }
}
