using Appli_Ticketing.Models;
using Appli_Ticketing.Views;
using System.Windows;

namespace Appli_Ticketing
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            _currentUser = user;

            if (_currentUser.IsAdmin)
                MainFrame.Navigate(new AdminDashboard());
            else
                MainFrame.Navigate(new UserDashboard(_currentUser.Id));
        }
    }

}
