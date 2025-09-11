using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using Appli_Ticketing.Models;
using Appli_Ticketing.Services;

namespace Appli_Ticketing.ViewModels
{
    public class DetailTicketViewModel : INotifyPropertyChanged
    {
        public Ticket Ticket { get; set; }

        public ICommand SaveCommand { get; }

        private readonly Action _closeAction;
        private readonly DatabaseService _db;

        public DetailTicketViewModel(Ticket ticket, Action closeAction)
        {
            Ticket = ticket;
            if (string.IsNullOrEmpty(Ticket.Type))
                Ticket.Type = "Incident";

            _closeAction = closeAction;
            _db = new DatabaseService();
            SaveCommand = new RelayCommand(Save);
        }


        private void Save()
        {
            if (Ticket == null)
            {
                System.Windows.MessageBox.Show("Aucun ticket sélectionné.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Ticket.Title) ||
                string.IsNullOrWhiteSpace(Ticket.Description) ||
                string.IsNullOrWhiteSpace(Ticket.Type) ||
                string.IsNullOrWhiteSpace(Ticket.Status))
            {
                System.Windows.MessageBox.Show("Certains champs obligatoires sont vides (Titre, Description, Type, Status).");
                return;
            }

            try
            {
                _db.UpdateTicket(Ticket);
                _closeAction?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erreur lors de la sauvegarde : " + ex.Message);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
