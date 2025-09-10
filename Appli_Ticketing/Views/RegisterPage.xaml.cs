using System.Windows;
using System.Windows.Controls;

namespace Appli_Ticketing.Views
{
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }


        private void OnBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
