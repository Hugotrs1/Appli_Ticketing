using Appli_Ticketing.Models;
using Dapper;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Appli_Ticketing.Views
{
    public partial class CreateTicket : Window
    {
        private readonly int _userId;
        private readonly DatabaseService _db = new();

        // On passe l'ID de l'utilisateur via le constructeur
        public CreateTicket(int userId)
        {
            InitializeComponent();
            LoadProblemes();
            _userId = userId;
        }

        private void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text.Trim();
            var desc = DescBox.Text.Trim();
            var type = (TypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            var probleme = ProblemeCombo.SelectedItem as Probleme;

            // Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(desc) ||
                string.IsNullOrWhiteSpace(type) ||
                probleme == null)
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var conn = _db.GetConnection();
            conn.Open();
            conn.Execute(
              @"INSERT INTO Tickets 
                 (Title, Description, Type, DateCreation, Status, UserId, ProblemeId) 
                VALUES 
                 (@t, @d, @ty, @dt, 'Ouvert', @u, @pid)",
              new
              {
                  t = title,
                  d = desc,
                  ty = type,
                  dt = DateTime.Now,
                  u = _userId,
                  pid = probleme.Id
              });

            MessageBox.Show("Ticket créé !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadProblemes()
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var problemes = conn.Query<Probleme>("SELECT * FROM Problemes");
            ProblemeCombo.ItemsSource = problemes;
        }
    }
}
