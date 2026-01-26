using Npgsql;

public class DatabaseService
{
    private readonly IConfiguration _config;

    public DatabaseService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> CanConnectAsync()
    {
        var connString = _config.GetConnectionString("SupabaseDb");

        try
        {
            using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
