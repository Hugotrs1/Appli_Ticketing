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

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _player = new MediaPlayer();

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets", "Sounds",
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
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show("Erreur inattendue : " + e.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                LogError(ex);
        }

        private void LogError(Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
                File.AppendAllText(logPath,
                    $"[{DateTime.Now}] {ex.GetType()} : {ex.Message}\n{ex.StackTrace}\n\n");
            }
            catch
            {
            }
        }
    }
}
