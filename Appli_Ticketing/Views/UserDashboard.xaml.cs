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
            DataContext = new UserDashboardViewModel(_userId);
        }

        private void OnCreateTicketClick(object sender, RoutedEventArgs e)
        {
            var createTicketWindow = new CreateTicket(_userId);
            createTicketWindow.ShowDialog();

            var viewModel = DataContext as UserDashboardViewModel;
            viewModel?.ReloadTickets();
        }
    }   
}
