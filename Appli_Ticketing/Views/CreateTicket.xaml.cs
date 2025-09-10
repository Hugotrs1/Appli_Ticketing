using Appli_Ticketing.Models;
using Appli_Ticketing.Services;
using Dapper;
using System;
using System.IO;
using System.Threading.Tasks;
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

        private void OnSubmitClick(object sender, RoutedEventArgs e)
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

            var ticket = new Ticket
            {
                Title = title,
                Description = desc,
                Type = type,
                DateCreation = DateTime.Now,
                Status = "Ouvert",
                UserId = _userId,
                ProblemName = probleme.Nom,        
                ProblemCriticite = probleme.Criticite
            };

            int newId = _db.AddTicket(ticket);


            string adminEmail = "trousselhugo@gmail.com";
            string sujet = "Nouveau ticket !";

            string corpsHtml = $@"
<!DOCTYPE html>
<html lang=""fr"">
<head>
    <meta charset=""UTF-8"">
    <title>Nouveau Ticket Créé</title>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f4f6f8;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background-color: #2E86C1;
            color: white;
            padding: 20px;
            text-align: center;
        }}
        .content {{ padding: 20px; }}
        .highlight {{
            background-color: #e7f3fe;
            border-left: 5px solid #2E86C1;
            padding: 15px;
            margin: 20px 0;
            border-radius: 6px;
        }}
        .criticite {{
            background-color: #fef5e7;
            border-left: 5px solid #f5b041;
            padding: 15px;
            margin: 20px 0;
            border-radius: 6px;
            color: #7d6608;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>🎫 Nouveau ticket créé</h2>
        </div>
        <div class=""content"">
            <p><strong>ID :</strong> {newId}</p>
            <p><strong>Titre :</strong> {title}</p>
            <div class=""highlight"">
                <p><strong>Description :</strong><br/>{desc}</p>
            </div>
            <div class=""criticite"">
                Criticité : {probleme.Criticite}
            </div>
            <img src=""cid:CriticiteImage"" alt=""Criticité"" />
        </div>
        <div class=""footer"">
            Appli Ticketing
        </div>
    </div>
</body>
</html>";

            string imagePath = "";
            if (probleme.Criticite < 30)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Alerte_Jaune.png");
            else if (probleme.Criticite < 70)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Alerte_Orange.png");
            else if (probleme.Criticite < 100)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Alerte_Rouge.png");
            else if (probleme.Criticite == 100)
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Alerte_Max.png");

            MessageBox.Show("Ticket créé — l'email est en cours d'envoi.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            _ = Task.Run(async () =>
            {
                if (File.Exists(imagePath))
                    await EmailService.SendAsync(adminEmail, sujet, corpsHtml, imagePath);
                else
                    await EmailService.SendAsync(adminEmail, sujet, corpsHtml);
            });

            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadProblemes()
        {
            ProblemeCombo.ItemsSource = _db.GetProblemes();
        }
    }
}
