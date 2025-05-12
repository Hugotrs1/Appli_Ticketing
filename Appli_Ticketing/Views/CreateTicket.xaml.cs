using System;
using System.Windows;
using System.Windows.Controls;
using Dapper;

namespace Appli_Ticketing.Views
{
    public partial class CreateTicket : Page
    {
        private readonly int _userId;
        private readonly DatabaseService _db = new();

        // on passe l'ID de l'utilisateur via le constructeur
        public CreateTicket(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text.Trim();
            var desc = DescBox.Text.Trim();
            var type = (TypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(desc) ||
                string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(
              @"INSERT INTO Tickets 
                 (Title, Description, Type, DateCreation, Status, UserId) 
                VALUES 
                 (@t, @d, @ty, @dt, 'Open', @u)",
              new
              {
                  t = title,
                  d = desc,
                  ty = type,
                  dt = DateTime.Now,
                  u = _userId
              });

            MessageBox.Show("Ticket créé !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            // Retour au dashboard
            this.NavigationService?.Navigate(new UserDashboard(_userId));
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new UserDashboard(_userId));
        }
    }
}
