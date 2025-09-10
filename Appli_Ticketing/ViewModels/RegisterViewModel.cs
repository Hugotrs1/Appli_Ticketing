using Appli_Ticketing.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;

public partial class RegisterViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string username;
    [ObservableProperty] private string password;
    [ObservableProperty] private bool isAdmin; 

    public RegisterViewModel()
    {
        _db = new DatabaseService();
    }

    [RelayCommand]
    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            System.Windows.MessageBox.Show("Tous les champs sont obligatoires.");
            return;
        }

        string hashedPwd = SecurityHelper.HashPassword(Password);

        using var conn = _db.GetConnection();
        conn.Open();
        conn.Execute(
            "INSERT INTO Users (Username, Password, IsAdmin) VALUES (@Username, @Password, @IsAdmin)",
            new { Username, Password = hashedPwd, IsAdmin = IsAdmin ? 1 : 0 });

        System.Windows.MessageBox.Show("Inscription réussie !");

        System.Windows.Application.Current.Windows
            .OfType<Appli_Ticketing.Views.RegisterPage>()
            .FirstOrDefault()?.Close();
    }
}
