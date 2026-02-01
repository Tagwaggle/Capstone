using LanceMudCapstone.DTOs;
using LanceMudCapstone.Services;
using Npgsql;

public static class UserApi
{
    public static void MapUserApi(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users");

        // GET /api/users
        users.MapGet("/", async (IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    "SELECT userid, username, email, isactive FROM users ORDER BY userid;",
                    conn
                );

                var reader = await cmd.ExecuteReaderAsync();

                var results = new List<UserDto>();

                while (await reader.ReadAsync())
                {
                    results.Add(new UserDto
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("isactive"))
                    });
                }

                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        users.MapGet("/Select/{uid:int}", async (int uid, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    "SELECT userid, username, email, isactive FROM users WHERE userid = @UserId;",
                    conn
                );

                cmd.Parameters.AddWithValue("@UserId", uid);

                var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var user = new UserDto
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("isactive"))
                    };
                    return Results.Ok(user);
                }

                return Results.NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });
        users.MapPut("/{uid:int}/password", async (int uid, PasswordDto dto, IConfiguration config) =>
        {
        var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var hashPassword = PasswordHelper.HashPassword(dto.Password);
                var cmd = new NpgsqlCommand(
                    "UPDATE users SET passwordhash = @hashPassword where userid = @userid", conn
                    );
                cmd.Parameters.AddWithValue("@hashPassword", hashPassword);
                cmd.Parameters.AddWithValue("@userid", uid);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {

                    return Results.Ok(new { Message = "Email updated successfully" });
                }

                return Results.NotFound(new { Message = "User not found" });
            }
           catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });
        users.MapPut("/{uid:int}/email", async (int uid, UserEmailDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    "UPDATE users SET email = @Email WHERE userid = @UserId;",
                    conn
                );

                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@UserId", uid);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {

                    return Results.Ok(new { Message = "Email updated successfully" });
                }

                return Results.NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

    }
}
