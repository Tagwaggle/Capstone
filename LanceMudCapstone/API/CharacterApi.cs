using LanceMudCapstone.DTOs;
using Npgsql;

namespace LanceMudCapstone.API;

// Character API endpoints
public static class CharacterApi
{
    public static void MapCharacterApi(this IEndpointRouteBuilder app)
    {
        var chars = app.MapGroup("/api/characters");

        chars.MapPost("/create", async (CreateCharacterDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"INSERT INTO characters 
                      (name, race, class, alignment, roomid)
                      VALUES (@name, @race, @class, @alignment, @roomid)
                      RETURNING characterid;",
                    conn
                );

                
                cmd.Parameters.AddWithValue("@name", dto.Name);
                cmd.Parameters.AddWithValue("@race", dto.Race);
                cmd.Parameters.AddWithValue("@class", dto.Class);
                cmd.Parameters.AddWithValue("@alignment", dto.Alignment);
                cmd.Parameters.AddWithValue("@roomid", dto.RoomId);

                var result = await cmd.ExecuteScalarAsync();

                if (result is int newId)
                    return Results.Ok(new { characterId = newId });

                return Results.Problem("Failed to create character.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        chars.MapPut("/update", async (UpdateCharacterDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var updates = new List<string>();
                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;

                void AddIfNotNull(string column, object? value)
                {
                    if (value != null)
                    {
                        var paramName = $"@{column}";
                        updates.Add($"{column} = {paramName}");
                        cmd.Parameters.AddWithValue(paramName, value);
                    }
                }

                AddIfNotNull("health", dto.Health);
                AddIfNotNull("maxhealth", dto.MaxHealth);
                AddIfNotNull("armorclass", dto.ArmorClass);
                AddIfNotNull("strength", dto.Strength);
                AddIfNotNull("dexterity", dto.Dexterity);
                AddIfNotNull("constitution", dto.Constitution);
                AddIfNotNull("intelligence", dto.Intelligence);
                AddIfNotNull("wisdom", dto.Wisdom);
                AddIfNotNull("charisma", dto.Charisma);
                AddIfNotNull("roomid", dto.RoomId);
                AddIfNotNull("isalive", dto.IsAlive);

                if (updates.Count == 0)
                    return Results.BadRequest(new ApiErrorDto { Message = "No fields to update." });

                cmd.CommandText =
                    $"UPDATE characters SET {string.Join(", ", updates)} WHERE characterid = @id;";
                cmd.Parameters.AddWithValue("@id", dto.CharacterId);

                var rows = await cmd.ExecuteNonQueryAsync();

                return rows > 0
                    ? Results.Ok()
                    : Results.BadRequest(new ApiErrorDto { Message = "Update failed" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        chars.MapGet("/user/{id}", async (int id, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"SELECT c.characterid, c.name, c.race, c.class, c.alignment, c.roomid
              FROM characters c
              JOIN playercharacters pc ON c.characterid = pc.characterid
              WHERE pc.userid = @uid;", conn);

                cmd.Parameters.AddWithValue("@uid", id);

                var reader = await cmd.ExecuteReaderAsync();
                var list = new List<CharacterDto>();
                while (await reader.ReadAsync())
                {
                    list.Add(new CharacterDto
                    {
                        CharacterId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Race = reader.GetString(2),
                        Class = reader.GetString(3),
                        ALignment = reader.GetString(4),
                        RoomId = reader.GetInt32(5)
                    });
                }
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in /api/characters/user/{id}: {ex}");
                return Results.Problem(ex.ToString());
            }
        });





    }
}
