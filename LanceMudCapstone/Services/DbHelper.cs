using Dapper;
using Npgsql;
using System.Data;


namespace LanceMudCapstone.Services;

public class DbHelper
{
    private readonly IConfiguration _config;

    public DbHelper(IConfiguration config)
    {
        _config = config;
    }

    public IDbConnection CreateConnection()
    {
        var connString = _config.GetConnectionString("SupabaseDb");
        return new NpgsqlConnection(connString);
    }
    public async Task<NpgsqlConnection> CreateOpenConnectionAsync()
    {
        var connString = _config.GetConnectionString("SupabaseDb");
        var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task<IEnumerable<T>> QuryListAsync<T>(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<T>(sql, parameters);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, parameters);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        var result = await conn.ExecuteScalarAsync<T>(sql, parameters);

        if (result == null) throw new Exception("ExecuteScalar returned null");

        return result;
    }


}
