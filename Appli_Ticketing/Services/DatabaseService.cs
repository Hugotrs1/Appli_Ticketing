using System.Data.SqlClient;
using Dapper;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=PC-HUGO\\mssqlserver01;Initial Catalog=Appli_Ticketing;Integrated Security=True;Encrypt=False";

    public SqlConnection GetConnection() => new SqlConnection(_connectionString);
}
