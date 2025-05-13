using System.Windows;
using Appli_Ticketing.Views;

namespace Appli_Ticketing
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginPage();
            loginWindow.Show();
        }
    }
}
