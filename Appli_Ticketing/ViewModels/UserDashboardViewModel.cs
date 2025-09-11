using Appli_Ticketing.Models;
using Appli_Ticketing.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Appli_Ticketing.ViewModels
{
    public partial class UserDashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _db;
        private readonly DispatcherTimer _timer;
        private readonly int _userId;

        [ObservableProperty] private ObservableCollection<Ticket> tickets;
        [ObservableProperty] private Ticket selectedTicket;

        public RelayCommand DeleteTicketCommand { get; }
        public RelayCommand ValidateTicketCommand { get; }
        public RelayCommand DetailCommand { get; }

        public bool CanDeleteTicket => SelectedTicket != null && string.IsNullOrEmpty(SelectedTicket.Response);
        public bool CanValidateTicket => SelectedTicket != null && !string.IsNullOrEmpty(SelectedTicket.Response);

        public UserDashboardViewModel(int userId)
        {
            _db = new DatabaseService();
            _userId = userId;
            LoadTickets();

            DeleteTicketCommand = new RelayCommand(DeleteTicket, () => CanDeleteTicket);
            ValidateTicketCommand = new RelayCommand(ValidateTicket, () => CanValidateTicket);
            DetailCommand = new RelayCommand(ShowDetails, () => SelectedTicket != null);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _timer.Tick += CheckTicketTimeouts;
            _timer.Start();
        }

        partial void OnSelectedTicketChanged(Ticket value)
        {
            OnPropertyChanged(nameof(CanDeleteTicket));
            OnPropertyChanged(nameof(CanValidateTicket));
            DetailCommand.NotifyCanExecuteChanged();
            DeleteTicketCommand.NotifyCanExecuteChanged();
            ValidateTicketCommand.NotifyCanExecuteChanged();
        }

        private void LoadTickets()
        {
            Tickets = new ObservableCollection<Ticket>(_db.GetTicketsByUser(_userId));
        }

        private void ShowDetails()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Aucun ticket sélectionné.");
                return;
            }

            var window = new Views.DetailTicket(SelectedTicket);
            if (Application.Current.MainWindow != window)
            {
                window.Owner = Application.Current.MainWindow;
            }

            window.ShowDialog();
        }


        private void DeleteTicket()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket avant de le supprimer.");
                return;
            }

            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer le ticket \"{SelectedTicket.Title}\" ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            _db.DeleteTicket(SelectedTicket.Id);
            ReloadTickets();
        }

        private void ValidateTicket()
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket avant de le valider.");
                return;
            }

            var result = MessageBox.Show(
                $"Voulez-vous valider le ticket \"{SelectedTicket.Title}\" ?",
                "Confirmation de validation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            SelectedTicket.Status = "Validé";
            _db.UpdateTicket(SelectedTicket);
            ReloadTickets();
        }

        private void CheckTicketTimeouts(object sender, EventArgs e)
        {
            var ouverts = _db.GetTicketsByUser(_userId).Where(t => t.Status == "Ouvert");
            foreach (var ticket in ouverts)
            {
                if ((DateTime.Now - ticket.DateCreation).TotalMinutes > 10)
                {
                    ticket.Status = "Expiré";
                    _db.UpdateTicket(ticket);
                }
            }
            ReloadTickets();
        }

        public void ReloadTickets()
        {
            LoadTickets();
            SelectedTicket = null;
        }
    }
}
