namespace LanceMudCapstone.Services;

public class SessionRepository
{
    private readonly SQLiteService _service;
    public SessionRepository(SQLiteService? service)
    {
        _service = service;
    }
    public bool InsertSession(string token, int userId)
    {
        DateTime storeDate = DateTime.UtcNow;
        DateTime expDate = DateTime.UtcNow.AddDays(7);

        using var conn = _service.GetConnection();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
INSERT INTO Sessions (Token, UserId, CreatedAt, ExpiresAt)
VALUES
(@token, @userId, @storeDate, @expDate);
";
        cmd.Parameters.AddWithValue("@token", token);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@storeDate", storeDate.ToString("O"));
        cmd.Parameters.AddWithValue("@expDate", expDate.ToString("O"));

        var rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }
    public int? GetUserIdFromToken(string token)
    {
        using var conn = _service.GetConnection();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
SELECT UserId
FROM Sessions
WHERE Token = @token;";
        cmd.Parameters.AddWithValue("@token", token);

        var user = cmd.ExecuteScalar();

        if (user == null || user == DBNull.Value) return null;

        return Convert.ToInt32(user); ;
    }
    public bool DeleteSession(string token)
    {
        using var conn = _service.GetConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
DELETE
FROM Sessions
WHERE Token = @token;";
        cmd.Parameters.AddWithValue("@token", token);
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
}
