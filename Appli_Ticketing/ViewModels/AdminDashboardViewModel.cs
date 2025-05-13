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
                ((RelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                ((RelayCommand)LogoutCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand RespondCommand { get; }
        public ICommand SetOnHoldCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminDashboardViewModel()
        {
            LoadTickets();
            RespondCommand = new RelayCommand(Respond, () => SelectedTicket != null);
            SetOnHoldCommand = new RelayCommand(SetOnHold, () => SelectedTicket != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedTicket != null);
            LogoutCommand = new RelayCommand(Logout);
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
                UpdateDb("Response = @r, Status = 'En attente'", new { r = SelectedTicket.Response });
                LoadTickets();
            }
        }

        private void SetOnHold()
        {
            if (SelectedTicket.Status != "Validé")
            {
                SelectedTicket.Status = "En attente";
                UpdateDb("Status = 'En attente'", null);
                LoadTickets();
            }
            else
            {
                MessageBox.Show("Vous ne pouvez pas mettre un ticket validé en attente.", "Action interdite", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete()
        {
            if (SelectedTicket.Status == "Validé")
            {
                MessageBox.Show("Impossible de supprimer un ticket validé. Il doit rester dans l'historique.", "Suppression refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Voulez-vous vraiment supprimer le ticket '{SelectedTicket.Title}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using var conn = new SqlConnection(
                    "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False");
                conn.Open();
                conn.Execute("DELETE FROM Tickets WHERE Id = @Id", new { Id = SelectedTicket.Id });

                LoadTickets();
            }
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

        private void Logout()
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

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
