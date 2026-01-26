using Dapper;
using Npgsql;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services;

public class UserService
{
    private readonly DbHelper _db;

    public UserService(DbHelper db)
    {
        _db = db;
    }

    public async Task<int> AddUserAsync(User user)
    {
        var sql = @"
            INSERT INTO users (username, passwordhash, email, isactive)
            VALUES (@UserName, @PasswordHash, @Email, @IsActive)
            RETURNING userid;
            ";

        return await _db.ExecuteScalarAsync<int>(sql, user);
    }
}
