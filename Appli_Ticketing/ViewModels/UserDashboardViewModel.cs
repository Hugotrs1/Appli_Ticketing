using Appli_Ticketing.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System.Collections.ObjectModel;
using System.Windows.Threading;

public partial class UserDashboardViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly DispatcherTimer _timer;
    private readonly int _userId;

    [ObservableProperty] private ObservableCollection<Ticket> tickets;

    public UserDashboardViewModel(int userId)
    {
        _db = new DatabaseService();
        _userId = userId;
        LoadTickets();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += CheckTicketTimeouts;
        _timer.Start();
    }

    private void LoadTickets()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        var result = conn.Query<Ticket>("SELECT * FROM Tickets WHERE UserId = @UserId", new { UserId = _userId });
        Tickets = new ObservableCollection<Ticket>(result);
    }

    private void CheckTicketTimeouts(object sender, EventArgs e)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        var result = conn.Query<Ticket>("SELECT * FROM Tickets WHERE Status = 'Open' AND UserId = @UserId", new { UserId = _userId });
        foreach (var ticket in result)
        {
            if ((DateTime.Now - ticket.DateCreation).TotalMinutes > 10)
            {
                conn.Execute("UPDATE Tickets SET Status = 'Expired' WHERE Id = @Id", new { ticket.Id });
            }
        }
    }

    [RelayCommand]
    private void DeleteTicket(Ticket ticket)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        conn.Execute("DELETE FROM Tickets WHERE Id = @Id", new { ticket.Id });
        LoadTickets();
    }
}
