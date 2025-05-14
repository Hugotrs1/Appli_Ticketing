using Appli_Ticketing.Models;
using Appli_Ticketing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Appli_Ticketing.Views
{
    public partial class DetailTicket : Window
    {
        public DetailTicket(Ticket ticket)
        {
            InitializeComponent();
            DataContext = new DetailTicketViewModel(ticket, this.Close);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
