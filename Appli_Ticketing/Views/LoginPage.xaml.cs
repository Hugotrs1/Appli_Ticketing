using System.Windows;
using System.Windows.Controls;
using Dapper;
using Appli_Ticketing.Views;
using System.Linq;
using Appli_Ticketing.Models;

namespace Appli_Ticketing.Views
{
    public partial class LoginPage : Page
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

            // validation basique
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

            // navigation vers le dashboard adapté
            if (this.NavigationService != null)
            {
                if (user.IsAdmin)
                    this.NavigationService.Navigate(new AdminDashboard());
                else
                    this.NavigationService.Navigate(new UserDashboard(user.Id));
            }
            else if (Application.Current.MainWindow is MainWindow wnd)
            {
                if (user.IsAdmin)
                    wnd.MainFrame.Navigate(new AdminDashboard());
                else
                    wnd.MainFrame.Navigate(new UserDashboard(user.Id));
            }
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
                this.NavigationService.Navigate(new RegisterPage());
            else if (Application.Current.MainWindow is MainWindow wnd)
                wnd.MainFrame.Navigate(new RegisterPage());
        }
    }
}
