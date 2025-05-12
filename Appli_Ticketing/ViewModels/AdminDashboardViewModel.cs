using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System.Data.SqlClient;
using System.Windows.Input;
using Appli_Ticketing.Models;
using Appli_Ticketing.Views;
using System.Windows;

namespace Appli_Ticketing.ViewModels
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Ticket> _tickets;
        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set
            {
                _tickets = value;
                OnPropertyChanged(nameof(Tickets));
            }
        }

        private Ticket _selectedTicket;
        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged(nameof(SelectedTicket));
                ((RelayCommand)RespondCommand).NotifyCanExecuteChanged();
                ((RelayCommand)SetOnHoldCommand).NotifyCanExecuteChanged();
                ((RelayCommand)ValidateCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand RespondCommand { get; }
        public ICommand SetOnHoldCommand { get; }
        public ICommand ValidateCommand { get; }


        public AdminDashboardViewModel()
        {
            LoadTickets();
            RespondCommand = new RelayCommand(Respond, () => SelectedTicket != null);
            SetOnHoldCommand = new RelayCommand(SetOnHold, () => SelectedTicket != null);
            ValidateCommand = new RelayCommand(Validate, () => SelectedTicket != null);
        }

        private void LoadTickets()
        {
            using var conn = new SqlConnection(
                "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False");
            conn.Open();
            var list = conn.Query<Ticket>("SELECT * FROM Tickets").ToList();
            Tickets = new ObservableCollection<Ticket>(list);
        }

        private void Respond()
        {
            var window = new ReponseAdmin();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true && !string.IsNullOrWhiteSpace(window.ResponseText))
            {
                SelectedTicket.Response = window.ResponseText;
                UpdateDb("Response = @r, Status = 'Closed'", new { r = SelectedTicket.Response });
                LoadTickets();
            }

        }
        private void SetOnHold()
        {
            SelectedTicket.Status = "En attente";
            UpdateDb("Status = 'En attente'", null);
            LoadTickets();
        }

        private void Validate()
        {
            SelectedTicket.Status = "Validé";
            UpdateDb("Status = 'Validé'", null);
            LoadTickets();
        }

        private void UpdateDb(string setClause, object param)
        {
            using var conn = new SqlConnection(
                "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False");
            conn.Open();
            conn.Execute($"UPDATE Tickets SET {setClause} WHERE Id = @Id",
                param is null
                    ? new { Id = SelectedTicket.Id }
                    : new { Id = SelectedTicket.Id, ((dynamic)param).r });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
