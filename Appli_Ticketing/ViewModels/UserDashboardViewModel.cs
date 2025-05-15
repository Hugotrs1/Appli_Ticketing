using Appli_Ticketing.Models;
using Appli_Ticketing.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Threading;

public partial class UserDashboardViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly DispatcherTimer _timer;
    private readonly int _userId;

    [ObservableProperty] private ObservableCollection<Ticket> tickets;
    [ObservableProperty] private Ticket selectedTicket;

    public RelayCommand DeleteTicketCommand { get; }
    public RelayCommand ValidateTicketCommand { get; }
    public RelayCommand DetailCommand { get; }


    public bool CanDeleteTicket => SelectedTicket != null && string.IsNullOrEmpty(SelectedTicket.Response);
    public bool CanValidateTicket => SelectedTicket != null && !string.IsNullOrEmpty(SelectedTicket.Response);

    public UserDashboardViewModel(int userId)
    {
        _db = new DatabaseService();
        _userId = userId;
        LoadTickets();

        DeleteTicketCommand = new RelayCommand(DeleteTicket, () => CanDeleteTicket);
        ValidateTicketCommand = new RelayCommand(ValidateTicket, () => CanValidateTicket);
        DetailCommand = new RelayCommand(ShowDetails, () => SelectedTicket != null);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += CheckTicketTimeouts;
        _timer.Start();
    }

    partial void OnSelectedTicketChanged(Ticket value)
    {
        OnPropertyChanged(nameof(CanDeleteTicket));
        OnPropertyChanged(nameof(CanValidateTicket));
        DetailCommand.NotifyCanExecuteChanged();
        DeleteTicketCommand.NotifyCanExecuteChanged();
        ValidateTicketCommand.NotifyCanExecuteChanged();
    }

    private void LoadTickets()
    {
        using var conn = new SqlConnection(
            "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False");
        conn.Open();
        var list = conn.Query<Ticket>(
            @"SELECT t.*, u.Username AS UserName, p.Nom AS ProblemName, p.Criticite AS ProblemCriticite 
          FROM Tickets t
          INNER JOIN Users u ON t.UserId = u.Id
          LEFT JOIN Problemes p ON t.ProblemeId = p.Id
          WHERE t.UserId = @UserId",
            new { UserId = _userId }
        ).ToList();

        Tickets = new ObservableCollection<Ticket>(list);
    }


    private void ShowDetails()
    {
        if (SelectedTicket == null)
        {
            MessageBox.Show("Aucun ticket sélectionné.");
            return;
        }

        var window = new DetailTicket(SelectedTicket)
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
    }


    private void DeleteTicket()
    {
        if (SelectedTicket == null) return;

        var result = MessageBox.Show(
            $"Voulez-vous vraiment supprimer le ticket \"{SelectedTicket.Title}\" ?",
            "Confirmation de suppression",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        using var conn = _db.GetConnection();
        conn.Open();
        conn.Execute("DELETE FROM Tickets WHERE Id = @Id", new { Id = SelectedTicket.Id });
        ReloadTickets();
    }

    private void ValidateTicket()
    {
        if (SelectedTicket == null) return;

        var result = MessageBox.Show(
            $"Voulez-vous valider le ticket \"{SelectedTicket.Title}\" ?",
            "Confirmation de validation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        using var conn = _db.GetConnection();
        conn.Open();
        conn.Execute("UPDATE Tickets SET Status = 'Validé' WHERE Id = @Id", new { Id = SelectedTicket.Id });
        ReloadTickets();
    }

    private void CheckTicketTimeouts(object sender, EventArgs e)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        var result = conn.Query<Ticket>("SELECT * FROM Tickets WHERE Status = 'Ouvert' AND UserId = @UserId", new { UserId = _userId });
        foreach (var ticket in result)
        {
            if ((DateTime.Now - ticket.DateCreation).TotalMinutes > 10)
            {
                conn.Execute("UPDATE Tickets SET Status = 'Expiré' WHERE Id = @Id", new { ticket.Id });
            }
        }
        ReloadTickets();
    }

    public void ReloadTickets()
    {
        LoadTickets();
    }
}
