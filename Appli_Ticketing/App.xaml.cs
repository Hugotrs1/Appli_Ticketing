using Appli_Ticketing.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Appli_Ticketing
{
    public partial class App : Application
    {
        private MediaPlayer _player;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialise le MediaPlayer
            _player = new MediaPlayer();

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets","Sounds",
                "dino_roar.wav");

            if (File.Exists(path))
            {
                _player.Open(new Uri(path, UriKind.Absolute));

                _player.MediaEnded += (s, _) =>
                {
                    _player.Position = TimeSpan.Zero;
                    _player.Play();
                };

                _player.Play();
            }
            else
            {
                MessageBox.Show($"Fichier audio introuvable : {path}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var loginWindow = new LoginPage();
            loginWindow.Show();
        }
    }
}
