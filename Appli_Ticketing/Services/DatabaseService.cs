using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using Appli_Ticketing.Models;

namespace Appli_Ticketing.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public SqlConnection GetConnection() => new SqlConnection(_connectionString);
        public IEnumerable<Ticket> GetTicketsByUser(int userId)
        {
            using var conn = GetConnection();
            return conn.Query<Ticket>(
                @"SELECT t.*, u.Username AS UserName
          FROM Tickets t
          INNER JOIN Users u ON t.UserId = u.Id
          WHERE t.UserId = @UserId",
                new { UserId = userId });
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            using var conn = GetConnection();
            return conn.Query<Ticket>(
                @"SELECT t.*, u.Username AS UserName
          FROM Tickets t
          INNER JOIN Users u ON t.UserId = u.Id");
        }


        public Ticket GetTicketById(int id)
        {
            using var conn = GetConnection();
            return conn.QuerySingleOrDefault<Ticket>(
                "SELECT * FROM Tickets WHERE Id = @Id", new { Id = id });
        }

        public int AddTicket(Ticket ticket)
        {
            using var conn = GetConnection();
            return conn.QuerySingle<int>(
                @"INSERT INTO Tickets 
          (Title, Description, Type, DateCreation, Status, UserId, ProblemName, ProblemCriticite)
          OUTPUT INSERTED.Id
          VALUES 
          (@Title, @Description, @Type, @DateCreation, @Status, @UserId, @ProblemName, @ProblemCriticite)",
                ticket);
        }


        public void UpdateTicket(Ticket ticket)
        {
            using var conn = GetConnection();
            conn.Execute(
                @"UPDATE Tickets 
                  SET Title=@Title, Description=@Description, Type=@Type, Response=@Response, Status=@Status 
                  WHERE Id=@Id",
                ticket);
        }

        public void DeleteTicket(int id)
        {
            using var conn = GetConnection();
            conn.Execute("DELETE FROM Tickets WHERE Id=@Id", new { Id = id });
        }
        public IEnumerable<User> GetAllUsers()
        {
            using var conn = GetConnection();
            return conn.Query<User>("SELECT * FROM Users");
        }

        public User GetUserById(int id)
        {
            using var conn = GetConnection();
            return conn.QuerySingleOrDefault<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
        }

        public int AddUser(User user)
        {
            using var conn = GetConnection();
            return conn.QuerySingle<int>(
                @"INSERT INTO Users (Username, Password, Email, IsAdmin)
                  OUTPUT INSERTED.Id
                  VALUES (@Username, @Password, @Email, @IsAdmin)",
                user);
        }

        public void UpdateUser(User user)
        {
            using var conn = GetConnection();
            conn.Execute(
                @"UPDATE Users 
                  SET Username=@Username, Password=@Password, Email=@Email, IsAdmin=@IsAdmin 
                  WHERE Id=@Id",
                user);
        }

        public void DeleteUser(int id)
        {
            using var conn = GetConnection();
            conn.Execute("DELETE FROM Users WHERE Id=@Id", new { Id = id });
        }

        public IEnumerable<Probleme> GetProblemes()
        {
            using var conn = GetConnection();
            return conn.Query<Probleme>("SELECT * FROM Problemes ORDER BY Criticite ASC");
        }
    }
}
