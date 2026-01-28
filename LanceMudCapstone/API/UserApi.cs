using LanceMudCapstone.DTOs;
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
    }
}
