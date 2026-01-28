using Npgsql;

public static class TestApi
{
    public static void MapTestApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/test-db", async (IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                return Results.Ok($"DB connection successful. ConnString={connString}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"DB connection failed. ConnString={connString}. Error={ex.Message}");
            }
        });

        app.MapGet("/api/test-db-users", async (IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand("SELECT * FROM users;", conn);
                var reader = await cmd.ExecuteReaderAsync();

                var results = new List<object>();

                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        UserId = reader["userid"],
                        Username = reader["username"],
                        Email = reader["email"],
                        Active = reader["isactive"]
                    });
                }

                return Results.Ok(new
                {
                    Message = "Query successful",
                    ConnString = connString,
                    Users = results
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Query failed. ConnString={connString}. Error={ex.Message}");
            }
        });
    }
}
