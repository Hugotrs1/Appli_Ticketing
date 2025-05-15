using Appli_Ticketing.Models;
using Appli_Ticketing.Services;
using Dapper;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Appli_Ticketing.Views
{
    public partial class CreateTicket : Window
    {
        private readonly int _userId;
        private readonly DatabaseService _db = new();

        public CreateTicket(int userId)
        {
            InitializeComponent();
            LoadProblemes();
            _userId = userId;
        }

        private async void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text.Trim();
            var desc = DescBox.Text.Trim();
            var type = (TypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            var probleme = ProblemeCombo.SelectedItem as Probleme;

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(desc) ||
                string.IsNullOrWhiteSpace(type) ||
                probleme == null)
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newId;

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                newId = conn.QuerySingle<int>(
                  @"INSERT INTO Tickets 
                     (Title, Description, Type, DateCreation, Status, UserId, ProblemeId)
                    OUTPUT INSERTED.Id
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
            }

            const string adminEmail = "admin@votreentreprise.com";
            string sujet = $"[Ticket #{newId}] Nouveau ticket";
            string corps = $@"
                Bonjour,

                Un nouveau ticket (ID {newId}) a été créé.

                Titre       : {title}
                Description : {desc}

                Cordialement.";

            await EmailService.SendAsync(adminEmail, sujet, corps);

            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dino.jpg");
            if (File.Exists(imagePath))
            {
                await EmailService.SendAsync(adminEmail, sujet, corps, imagePath);
            }

            MessageBox.Show("Ticket créé et mail envoyé à l’administrateur.", "Succès",
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
