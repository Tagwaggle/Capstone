namespace LanceMudCapstone.Services;

using Microsoft.Data.Sqlite;

public class SQLiteService
{
    private readonly string _connectionString;

    public SQLiteService(IWebHostEnvironment env)
    {
        var dbPath = Path.Combine(env.ContentRootPath, "Data", "sessions.db");

        _connectionString = $"Data Source={dbPath}";
    }

    public SqliteConnection GetConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
