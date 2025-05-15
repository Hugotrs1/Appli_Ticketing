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

            const string adminEmail = "trousselhugo@gmail.com";
            string sujet = $"Nouveau ticket !";

            string corpsHtml = $@"
<!DOCTYPE html>
<html lang=""fr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Nouveau Ticket Créé</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }}
        h2 {{
            color: #2E86C1;
            border-bottom: 2px solid #2E86C1;
            padding-bottom: 10px;
        }}
        p {{
            color: #333;
            line-height: 1.6;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 0.9em;
            color: #777;
        }}
        .highlight {{
            background-color: #e7f3fe;
            border-left: 4px solid #2E86C1;
            padding: 10px;
            margin: 10px 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Nouveau ticket créé</h2>
        <p><strong>ID :</strong> {newId}</p>
        <p><strong>Titre :</strong> {title}</p>
        <div class=""highlight"">
            <p><strong>Description :</strong><br/>{desc}</p>
        </div>
        <p><strong>Criticité :</strong> {probleme.Criticite}</p>
        <div class=""footer"">
            <p><strong>Cordialement,<br/>Appli Ticketing</strong></p>
        </div>
    </div>
</body>
</html>
>";

            var imagePath = "";
            if (probleme.Criticite < 30)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faible.png");
            else if (probleme.Criticite < 70)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "moyen.png");
            else
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "eleve.png");

            if (File.Exists(imagePath))
                await EmailService.SendAsync(adminEmail, sujet, corpsHtml, imagePath);
            else
                await EmailService.SendAsync(adminEmail, sujet, corpsHtml);

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
