using Appli_Ticketing;
using Appli_Ticketing.Models;
using Appli_Ticketing.Services;
using Appli_Ticketing.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System.Windows;
using System.Windows.Input;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string username;
    [ObservableProperty] private string password;

    private readonly DatabaseService _db;

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    public LoginViewModel()
    {
        _db = new DatabaseService();
        LoginCommand = new RelayCommand(Login);
        RegisterCommand = new RelayCommand(OpenRegisterWindow);
    }

    private void OpenRegisterWindow()
    {
        var registerWindow = new RegisterPage();
        registerWindow.Owner = Application.Current.MainWindow;
        registerWindow.ShowDialog();
    }


    private void Login()
    {
        var user = Authenticate();
        if (user != null)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Show();

            Application.Current.Windows[0]?.Close();
        }
        else
        {
            MessageBox.Show("Identifiants incorrects.");
        }
    }

    private User Authenticate()
    {
        string hashedPwd = SecurityHelper.HashPassword(Password);

        using var conn = _db.GetConnection();
        conn.Open();
        var user = conn.QueryFirstOrDefault<User>(
            "SELECT * FROM Users WHERE Username = @Username AND Password = @Password",
            new { Username = username, Password = hashedPwd });
        return user;
    }
}
