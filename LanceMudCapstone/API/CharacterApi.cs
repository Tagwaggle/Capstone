using LanceMudCapstone.DTOs;
using Npgsql;

namespace LanceMudCapstone.API;

public static class CharacterApi
{
    public static void MapCharacterApi(this IEndpointRouteBuilder app)
    {
        var chars = app.MapGroup("/api/characters");

        // CREATE CHARACTER
        chars.MapPost("/create", async (CreateCharacterDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"INSERT INTO characters 
                      (userid, name, race, class, alignment)
                      VALUES (@uid, @name, @race, @class, @alignment)
                      RETURNING characterid;",
                    conn
                );

                cmd.Parameters.AddWithValue("@uid", dto.UserId);
                cmd.Parameters.AddWithValue("@name", dto.Name);
                cmd.Parameters.AddWithValue("@race", dto.Race);
                cmd.Parameters.AddWithValue("@class", dto.Class);
                cmd.Parameters.AddWithValue("@alignment", dto.Alignment);

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

        // UPDATE CHARACTER (PATCH-STYLE)
        chars.MapPut("/update", async (UpdateCharacterDto dto, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                // Build dynamic SQL for only the fields that are not null
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

        // GET CHARACTER BY ID
        chars.MapGet("/{id:int}", async (int id, IConfiguration config) =>
        {
            var connString = config["SupabaseDb"];

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"SELECT characterid, name, race, class, level, alignment,
                             health, maxhealth, armorclass,
                             strength, dexterity, constitution, intelligence, wisdom, charisma,
                             hitdice, roomid, isalive, charactertype
                      FROM characters
                      WHERE characterid = @id;",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);

                var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return Results.NotFound(new ApiErrorDto { Message = "Character not found" });

                var dto = new CharacterDto
                {
                    CharacterId = reader.GetInt32(reader.GetOrdinal("characterid")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Race = reader.GetString(reader.GetOrdinal("race")),
                    Class = reader.GetString(reader.GetOrdinal("class")),
                    Level = reader.GetInt32(reader.GetOrdinal("level")),
                    ALignment = reader.GetString(reader.GetOrdinal("alignment")),

                    Health = reader.GetInt32(reader.GetOrdinal("health")),
                    MaxHealth = reader.GetInt32(reader.GetOrdinal("maxhealth")),
                    ArmorClass = reader.GetInt32(reader.GetOrdinal("armorclass")),

                    Strength = reader.GetInt32(reader.GetOrdinal("strength")),
                    Dexterity = reader.GetInt32(reader.GetOrdinal("dexterity")),
                    Constitution = reader.GetInt32(reader.GetOrdinal("constitution")),
                    Intelligence = reader.GetInt32(reader.GetOrdinal("intelligence")),
                    Wisdom = reader.GetInt32(reader.GetOrdinal("wisdom")),
                    Charisma = reader.GetInt32(reader.GetOrdinal("charisma")),

                    HitDice = reader.GetString(reader.GetOrdinal("hitdice")),
                    RoomId = reader.GetInt32(reader.GetOrdinal("roomid")),
                    isAlive = reader.GetBoolean(reader.GetOrdinal("isalive")),
                    CharacterType = reader.GetString(reader.GetOrdinal("charactertype"))
                };

                return Results.Ok(dto);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });
    }
}
