using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;

public partial class RegisterViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string username;
    [ObservableProperty] private string password;

    public RegisterViewModel()
    {
        _db = new DatabaseService();
    }

    [RelayCommand]
    private void Register()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        conn.Execute(
            "INSERT INTO Users (Username, Password, IsAdmin) VALUES (@Username, @Password, 0)",
            new { Username, Password });
    }
}
