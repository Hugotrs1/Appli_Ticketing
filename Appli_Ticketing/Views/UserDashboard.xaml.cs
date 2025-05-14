using System.Windows;
using System.Windows.Controls;

namespace Appli_Ticketing.Views
{
    public partial class UserDashboard : Page
    {
        private readonly int _userId;

        public UserDashboard(int userId)
        {
            InitializeComponent();
            _userId = userId;
            var viewModel = new UserDashboardViewModel(_userId);
            DataContext = viewModel;
        }


        private void OnCreateTicketClick(object sender, RoutedEventArgs e)
        {
            var createTicketWindow = new CreateTicket(_userId);
            createTicketWindow.ShowDialog();

            var viewModel = DataContext as UserDashboardViewModel;
            viewModel?.ReloadTickets();
        }
        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var loginPage = new LoginPage();
            loginPage.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
