using System.Windows;
using System.Windows.Controls;
using Dapper;
using Appli_Ticketing.Views;
using System.Linq;
using Appli_Ticketing.Models;

namespace Appli_Ticketing.Views
{
    public partial class LoginPage : Window
    {
        private readonly DatabaseService _db = new();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var userName = UsernameTextBox.Text.Trim();
            var pwd = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(pwd))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var conn = _db.GetConnection();
            conn.Open();
            var user = conn
                .Query<User>("SELECT * FROM Users WHERE Username = @u AND Password = @p",
                             new { u = userName, p = pwd })
                .FirstOrDefault();

            if (user == null)
            {
                MessageBox.Show("Identifiants incorrects.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OnLoginSuccess(user);
        }

        private void OnLoginSuccess(User user)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterPage();
            registerWindow.ShowDialog();
        }
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
