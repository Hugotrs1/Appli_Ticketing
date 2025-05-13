using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Appli_Ticketing.Views;
using Dapper;
using Appli_Ticketing;
using System.Windows.Input;
using Appli_Ticketing.Models;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string username;
    [ObservableProperty] private string password;

    private readonly DatabaseService _db;

    public ICommand LoginCommand { get; }

    public ICommand RegisterRedirectCommand { get; }

    public LoginViewModel()
    {
        _db = new DatabaseService();

        // Initialisation des commandes
        LoginCommand = new RelayCommand(Login);
        RegisterRedirectCommand = new RelayCommand(RedirectToRegisterPage);
    }

    private void Login()
    {
        var user = Authenticate();
        if (user != null)
        {
        }
        else
        {
            MessageBox.Show("Identifiants incorrects.");
        }
    }

    private User Authenticate()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        var user = conn.QueryFirstOrDefault<User>(
            "SELECT * FROM Users WHERE Username = @Username AND Password = @Password",
            new { Username = username, Password = password });
        return user;
    }
    private void RedirectToRegisterPage()
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        mainWindow?.MainFrame.Navigate(new RegisterPage());
    }
}
