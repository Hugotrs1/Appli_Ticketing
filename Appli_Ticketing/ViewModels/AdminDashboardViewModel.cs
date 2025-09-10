using Appli_Ticketing.Models;
using Appli_Ticketing.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Appli_Ticketing.ViewModels
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;

        private ObservableCollection<Ticket> _tickets;
        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set { _tickets = value; OnPropertyChanged(nameof(Tickets)); }
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
                ((RelayCommand)DetailCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand RespondCommand { get; }
        public ICommand SetOnHoldCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand DetailCommand { get; }

        public AdminDashboardViewModel()
        {
            _db = new DatabaseService();
            LoadTickets();

            RespondCommand = new RelayCommand(Respond, () => SelectedTicket != null);
            SetOnHoldCommand = new RelayCommand(SetOnHold, () => SelectedTicket != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedTicket != null);
            LogoutCommand = new RelayCommand(Logout);
            DetailCommand = new RelayCommand(ShowDetails, () => SelectedTicket != null);
        }

        private void LoadTickets()
        {
            Tickets = new ObservableCollection<Ticket>(_db.GetAllTickets());
        }

        private void Respond()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket avant de répondre.");
                return;
            }

            var window = new Views.ReponseAdmin();
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true && !string.IsNullOrWhiteSpace(window.ResponseText))
            {
                SelectedTicket.Response = window.ResponseText;
                SelectedTicket.Status = "En attente";
                _db.UpdateTicket(SelectedTicket);

                LoadTickets();
                SelectedTicket = null;
            }
        }

        private void SetOnHold()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket avant de le mettre en attente.");
                return;
            }

            if (SelectedTicket.Status != "Validé")
            {
                SelectedTicket.Status = "En attente";
                _db.UpdateTicket(SelectedTicket);
                LoadTickets();
                SelectedTicket = null;
            }
            else
            {
                MessageBox.Show("Vous ne pouvez pas mettre un ticket validé en attente.",
                                "Action interdite", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket avant de le supprimer.");
                return;
            }

            if (SelectedTicket.Status == "Validé")
            {
                MessageBox.Show("Impossible de supprimer un ticket validé. Il doit rester dans l'historique.",
                                "Suppression refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Voulez-vous vraiment supprimer le ticket '{SelectedTicket.Title}' ?",
                                         "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _db.DeleteTicket(SelectedTicket.Id);
                LoadTickets();
                SelectedTicket = null;
            }
        }

        private void ShowDetails()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket pour voir les détails.");
                return;
            }

            var window = new Views.DetailTicket(SelectedTicket)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        private void Logout()
        {
            var loginPage = new Views.LoginPage();
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
