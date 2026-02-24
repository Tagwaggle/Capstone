using Dapper;
using LanceMudCapstone.Enums;
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
        using var conn = _db.CreateConnection();

        var sql = @"
            INSERT INTO users (username, passwordhash, email, isactive)
            VALUES (@Username, @PasswordHash, @Email, @IsActive)
            RETURNING userid;
        ";

        return await conn.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var conn = _db.CreateConnection();

        var sql = @"SELECT * FROM users WHERE username = @username LIMIT 1;";

        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { username });
    }

    public async Task<User?> ValidateLoginAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);

        if (user == null) return null;
        if (!PasswordHelper.VerifyPassword(password, user.PasswordHash)) return null;

        return user;
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();

        var sql = @"SELECT * FROM users WHERE userid = @id LIMIT 1;";

        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { id });
    }
    public async Task<bool> UpdateUserIconAsync(int userId, UserIcon icon)
    {
        using var conn = _db.CreateConnection();

        var sql = @"UPDATE users 
                SET profilepictureid = @icon 
                WHERE userid = @id;";

        var rows = await conn.ExecuteAsync(sql, new
        {
            id = userId,
            icon = (int)icon
        });

        return rows > 0;
    }

}