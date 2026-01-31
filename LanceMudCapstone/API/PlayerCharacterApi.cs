using LanceMudCapstone.DTOs;
using Npgsql;

namespace LanceMudCapstone.API;

public static class PlayerCharacterApi
{
    public static void MapPlayerCharacterApi(this IEndpointRouteBuilder app)
    {
        // Create a route group under /api/playercharacters
        var pcs = app.MapGroup("/api/playercharacters");

        // POST /api/playercharacters/create
        pcs.MapPost("/create", async (CreatePlayerCharacterDto dto, IConfiguration config) =>
        {
            Console.WriteLine($"Incoming body: UserId={dto.UserId}, CharacterId={dto.CharacterId}, Gold={dto.Gold}");

            var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"INSERT INTO playercharacters (userid, characterid, gold, activequest, lastonline, createdat)
                      VALUES (@uid, @cid, @gold, @quest, @last, NOW())
                      RETURNING playercharacterid;",
                    conn
                );

                cmd.Parameters.AddWithValue("@uid", dto.UserId);
                cmd.Parameters.AddWithValue("@cid", dto.CharacterId);
                cmd.Parameters.AddWithValue("@gold", dto.Gold);
                cmd.Parameters.AddWithValue("@quest", (object?)dto.ActiveQuest ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@last", (object?)dto.LastOnline ?? DBNull.Value);

                var result = await cmd.ExecuteScalarAsync();
                if (result is int newId)
                    return Results.Ok(new { playerCharacterId = newId });

                return Results.Problem("Failed to create playercharacter.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });
    }
}
