using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Input;
using Appli_Ticketing.Models;
using Dapper;

namespace Appli_Ticketing.ViewModels
{
    public class DetailTicketViewModel : INotifyPropertyChanged
    {
        public Ticket Ticket { get; set; }

        public ICommand SaveCommand { get; }

        private readonly Action _closeAction;

        public DetailTicketViewModel(Ticket ticket, Action closeAction)
        {
            Ticket = ticket;
            _closeAction = closeAction;
            SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            using var conn = new SqlConnection(
                "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False");
            conn.Open();

            conn.Execute("UPDATE Tickets SET Title = @Title, Description = @Description, Type = @Type WHERE Id = @Id", Ticket);

            _closeAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
