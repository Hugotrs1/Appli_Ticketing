using System.Windows;
using System.Windows.Controls;
using Appli_Ticketing.Models;
using Dapper;

namespace Appli_Ticketing.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void OnRegisterConfirm(object sender, RoutedEventArgs e)
        {
            var user = UsernameTextBox.Text;
            var pass = PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur");
                return;
            }

            using var conn = new DatabaseService().GetConnection();
            conn.Open();
            var exists = conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { Username = user });
            if (exists != null)
            {
                MessageBox.Show("Nom déjà pris.", "Erreur");
                return;
            }
            conn.Execute("INSERT INTO Users (Username, Password, IsAdmin) VALUES (@Username, @Password, 0)",
                         new { Username = user, Password = pass });

            MessageBox.Show("Inscription réussie !", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            this.NavigationService?.Navigate(new LoginPage());
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }
    }
}
