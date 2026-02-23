namespace LanceMudCapstone.Services;

using Microsoft.Data.Sqlite;

public class SQLiteService
{
    private readonly string _connectionString;

    public SQLiteService(IWebHostEnvironment env)
    {
        var dataDir = "/var/data";

        if (!Directory.Exists(dataDir)) { Directory.CreateDirectory(dataDir); }

        var dbPath = Path.Combine(dataDir, "sessions.db");

        _connectionString = $"Data Source={dbPath}";
    }

    public SqliteConnection GetConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
