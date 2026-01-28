using LanceMudCapstone.DTOs;
using LanceMudCapstone.Services;
using Npgsql;

public static class UserApi
{
    public static void MapUserApi(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users");

        users.MapGet("/", async (DbHelper db) =>
        {
            var sql = "SELECT userid, username, email, isactive FROM users ORDER BY userid;";
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
        });

        users.MapGet("/{id:int}", async (int id, UserService svc) =>
        {
            var user = await svc.GetUserByIdAsync(id);
            if (user is null)
                return Results.NotFound(new ApiErrorDto { Message = "User not found" });

            return Results.Ok(new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            });
        });
    }
}
