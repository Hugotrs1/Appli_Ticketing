using Appli_Ticketing.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Appli_Ticketing.Views
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
    }
}
