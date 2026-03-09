using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;

namespace LanceMudCapstone.API;



    // Character API endpoints
    public static class CharacterApi
    {
        public static void MapCharacterApi(this IEndpointRouteBuilder app)
        {
            var chars = app.MapGroup("/api/characters");

            chars.MapPost("/create", async (CharacterDto dto, IConfiguration config) =>
            {
                var connString = config["SupabaseDb"];
                try
                {
                    await using var conn = new NpgsqlConnection(connString);
                    await conn.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        @"INSERT INTO characters
                    (
                        name, race, class, alignment,
                        strength, dexterity, constitution, intelligence, wisdom, charisma,
                        maxhealth, health, armorclass,
                        hitdice, roomid, isalive, charactertype, level
                    )
                    VALUES
                    (
                        @Name, @Race, @Class, @Alignment,
                        @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma,
                        @MaxHealth, @Health, @ArmorClass,
                        @HitDice, @RoomId, true, 'pc', 1
                    )
                    RETURNING characterid;"
                        ,
                        conn
                    );


                    cmd.Parameters.AddWithValue("@Strength", dto.Strength);
                    cmd.Parameters.AddWithValue("@Dexterity", dto.Dexterity);
                    cmd.Parameters.AddWithValue("@Constitution", dto.Constitution);
                    cmd.Parameters.AddWithValue("@Intelligence", dto.Intelligence);
                    cmd.Parameters.AddWithValue("@Wisdom", dto.Wisdom);
                    cmd.Parameters.AddWithValue("@Charisma", dto.Charisma);

                    cmd.Parameters.AddWithValue("@MaxHealth", dto.MaxHealth);
                    cmd.Parameters.AddWithValue("@Health", dto.MaxHealth); // starting HP = max
                    cmd.Parameters.AddWithValue("@ArmorClass", dto.ArmorClass);
                    cmd.Parameters.AddWithValue("@HitDice", dto.HitDice);

                    cmd.Parameters.AddWithValue("@RoomId", dto.RoomId);

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
            chars.MapGet("/pfile/{id:int}", async (int id, IConfiguration config) =>
            {
                var connString = config["SupabaseDb"];
                try
                {
                    await using var conn = new NpgsqlConnection(connString);
                    await conn.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        @"SELECT pc.playercharacterid, pc.userid, pc.characterid,
                     c.name, c.race, c.class, c.level, c.alignment, c.health, c.maxhealth,
                     c.armorclass, c.strength, c.dexterity, c.constitution, c.intelligence,
                     c.wisdom, c.charisma, c.hitdice, c.roomid, c.isalive, c.charactertype,
                     pc.gold, pc.activequest, pc.lastonline, pc.createdat
              FROM playercharacters pc
              JOIN characters c ON pc.characterid = c.characterid
              WHERE pc.characterid = @cid;", conn);

                    cmd.Parameters.AddWithValue("@cid", id);

                    await using var reader = await cmd.ExecuteReaderAsync();
                    PlayerCharacterDto? _pfile = null;

                    if (await reader.ReadAsync())
                    {
                        _pfile = new PlayerCharacterDto
                        {
                            PlayerCharacterId = reader.GetInt32(reader.GetOrdinal("playercharacterid")),
                            UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                            CharacterId = reader.GetInt32(reader.GetOrdinal("characterid")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Race = reader.IsDBNull(reader.GetOrdinal("race")) ? null : reader.GetString(reader.GetOrdinal("race")),
                            Class = reader.IsDBNull(reader.GetOrdinal("class")) ? null : reader.GetString(reader.GetOrdinal("class")),
                            Level = reader.GetInt32(reader.GetOrdinal("level")),
                            Alignment = reader.IsDBNull(reader.GetOrdinal("alignment")) ? null : reader.GetString(reader.GetOrdinal("alignment")),
                            Health = reader.GetInt32(reader.GetOrdinal("health")),
                            MaxHealth = reader.GetInt32(reader.GetOrdinal("maxhealth")),
                            ArmorClass = reader.GetInt32(reader.GetOrdinal("armorclass")),
                            Strength = reader.GetInt32(reader.GetOrdinal("strength")),
                            Dexterity = reader.GetInt32(reader.GetOrdinal("dexterity")),
                            Constitution = reader.GetInt32(reader.GetOrdinal("constitution")),
                            Intelligence = reader.GetInt32(reader.GetOrdinal("intelligence")),
                            Wisdom = reader.GetInt32(reader.GetOrdinal("wisdom")),
                            Charisma = reader.GetInt32(reader.GetOrdinal("charisma")),
                            HitDice = reader.IsDBNull(reader.GetOrdinal("hitdice")) ? null : reader.GetString(reader.GetOrdinal("hitdice")),
                            RoomId = reader.GetInt32(reader.GetOrdinal("roomid")),
                            IsAlive = reader.GetBoolean(reader.GetOrdinal("isalive")),
                            CharacterType = reader.IsDBNull(reader.GetOrdinal("charactertype")) ? null : reader.GetString(reader.GetOrdinal("charactertype")),
                            Gold = reader.GetInt32(reader.GetOrdinal("gold")),
                            ActiveQuest = reader.IsDBNull(reader.GetOrdinal("activequest")) ? null : reader.GetInt32(reader.GetOrdinal("activequest")),
                            LastOnline = reader.IsDBNull(reader.GetOrdinal("lastonline")) ? null : reader.GetDateTime(reader.GetOrdinal("lastonline")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
                        };

                    }

                    return _pfile != null ? Results.Ok(_pfile) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in /api/characters/pfile/{id}: {ex}");
                    return Results.Problem(ex.ToString());
                }
            });
            chars.MapGet("/sheet/{id:int}", async (int id, IConfiguration config, CharacterSheetService sheetService) =>
            {
                var connString = config["SupabaseDb"];
                try
                {
                    await using var conn = new NpgsqlConnection(connString);
                    await conn.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        @"SELECT pc.playercharacterid, pc.userid, pc.characterid,
                     c.name, c.race, c.class, c.level, c.alignment, c.health, c.maxhealth,
                     c.armorclass, c.strength, c.dexterity, c.constitution, c.intelligence,
                     c.wisdom, c.charisma, c.hitdice, c.roomid, c.isalive, c.charactertype,
                     pc.gold, pc.activequest, pc.lastonline, pc.createdat, pc.xp, pc.xpneeded,
                     u.username
                FROM playercharacters pc
                JOIN characters c ON pc.characterid = c.characterid
                JOIN users u ON pc.userid = u.userid
                WHERE pc.characterid = @cid;", conn);

                    cmd.Parameters.AddWithValue(@"cid", id);

                    await using var reader = await cmd.ExecuteReaderAsync();

                    if (!await reader.ReadAsync())
                        return Results.NotFound();
                    var pfile = new PlayerCharacterDto
                    {
                        PlayerCharacterId = reader.GetInt32(reader.GetOrdinal("playercharacterid")),
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        CharacterId = reader.GetInt32(reader.GetOrdinal("characterid")),
                        Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
                        Race = reader.IsDBNull(reader.GetOrdinal("race")) ? null : reader.GetString(reader.GetOrdinal("race")),
                        Class = reader.IsDBNull(reader.GetOrdinal("class")) ? null : reader.GetString(reader.GetOrdinal("class")),
                        Level = reader.GetInt32(reader.GetOrdinal("level")),
                        Alignment = reader.IsDBNull(reader.GetOrdinal("alignment")) ? null : reader.GetString(reader.GetOrdinal("alignment")),
                        Health = reader.GetInt32(reader.GetOrdinal("health")),
                        MaxHealth = reader.GetInt32(reader.GetOrdinal("maxhealth")),
                        ArmorClass = reader.GetInt32(reader.GetOrdinal("armorclass")),
                        Strength = reader.GetInt32(reader.GetOrdinal("strength")),
                        Dexterity = reader.GetInt32(reader.GetOrdinal("dexterity")),
                        Constitution = reader.GetInt32(reader.GetOrdinal("constitution")),
                        Intelligence = reader.GetInt32(reader.GetOrdinal("intelligence")),
                        Wisdom = reader.GetInt32(reader.GetOrdinal("wisdom")),
                        Charisma = reader.GetInt32(reader.GetOrdinal("charisma")),
                        HitDice = reader.IsDBNull(reader.GetOrdinal("hitdice")) ? null : reader.GetString(reader.GetOrdinal("hitdice")),
                        RoomId = reader.GetInt32(reader.GetOrdinal("roomid")),
                        IsAlive = reader.GetBoolean(reader.GetOrdinal("isalive")),
                        CharacterType = reader.IsDBNull(reader.GetOrdinal("charactertype")) ? null : reader.GetString(reader.GetOrdinal("charactertype")),
                        Gold = reader.GetInt32(reader.GetOrdinal("gold")),
                        ActiveQuest = reader.IsDBNull(reader.GetOrdinal("activequest")) ? null : reader.GetInt32(reader.GetOrdinal("activequest")),
                        LastOnline = reader.IsDBNull(reader.GetOrdinal("lastonline")) ? null : reader.GetDateTime(reader.GetOrdinal("lastonline")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Xp = reader.GetInt32(reader.GetOrdinal("xp")),
                        XpNeeded = reader.GetInt32(reader.GetOrdinal("xpneeded"))
                    };

                    var pdfBytes = sheetService.GenerateCharacterSheet(pfile);

                    return Results.File(
                        pdfBytes,
                        contentType: "application/pdf",
                        fileDownloadName: $"{pfile.Name}_CharacterSheet.pdf"
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in /api/characters/sheet/{id}: {ex}");
                    return Results.Problem(ex.ToString());
                }
            });
        }
    }
