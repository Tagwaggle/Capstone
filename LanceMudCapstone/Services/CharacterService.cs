using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LanceMudCapstone.Services;

public class CharacterService : ICharacterService
{
    private readonly DbHelper _db;

    public CharacterService(DbHelper db)
    {
        _db = db;
    }

    public async Task<int> CreateCharacterAsync(PlayerCharacterDto dto)
    {
        await using var conn = await _db.CreateOpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();
        if ((dto.RoomId == 1) && (dto.CharacterType != "IMM"))
        {
            dto.RoomId = 2;
        }
        const string insertCharacterSql = @"
        INSERT INTO characters
        (
            name, race, ""class"", alignment,
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
        RETURNING characterid;
    ";

        var characterId = await conn.ExecuteScalarAsync<int>(insertCharacterSql, new
        {
            dto.Name,
            dto.Race,
            dto.Class,
            dto.Alignment,

            dto.Strength,
            dto.Dexterity,
            dto.Constitution,
            dto.Intelligence,
            dto.Wisdom,
            dto.Charisma,

            dto.MaxHealth,
            Health = dto.MaxHealth,
            dto.ArmorClass,
            dto.HitDice,

            dto.RoomId
        }, tx);

        // 2. Insert into playercharacters
        const string insertPlayerSql = @"
        INSERT INTO playercharacters
        (characterid, userid, gold, activequest, lastonline, mana, maxmana, stamina, maxstamina, xp, xpneeded)
        VALUES
        (@CharacterId, @UserId, @Gold, @ActiveQuest, @LastOnline, @Mana, @MaxMana, @Stamina, @MaxStamina, @Xp, @XpNeeded);
    ";

        await conn.ExecuteAsync(insertPlayerSql, new
        {
            CharacterId = characterId,
            dto.UserId,
            dto.Gold,
            dto.ActiveQuest,
            dto.LastOnline,
            dto.Mana,
            dto.MaxMana,
            dto.Stamina,
            dto.MaxStamina,
            dto.Xp,
            dto.XpNeeded
        }, tx);

        await tx.CommitAsync();
        return characterId;
    }

    public Task RecalculateStats(int playerCharacterId)
    {
        return Task.CompletedTask;
    }


    public Task<bool> UpdateCharacterAsync(UpdateCharacterDto dto)
        => Task.FromResult(true);

    public Task<CharacterDto?> GetCharacterByIdAsync(int id)
        => Task.FromResult<CharacterDto?>(null);
    public async Task<PlayerCharacterDto> SavePlayer(PlayerCharacterDto saveState)
    {
        await using var conn = await _db.CreateOpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();

        const string updateCharacterSql = @"
            UPDATE characters
            SET
                name = @Name, race = @Race, alignment = @Alignment, health = @Health, maxhealth = @MaxHealth, armorclass = @ArmorClass,
                strength = @Strength, dexterity = @Dexterity, constitution = @Constitution, intelligence = @Intelligence, wisdom = @Wisdom,
                charisma = @Charisma, roomid = @RoomId
            WHERE
                characterid = @CharacterId;";
        // const string updatePlayerSql = @"
        //     UPDATE playercharacters
        //     SET
        //         
        //     WHERE
        //         playercharacterid = @PlayerCharacterId;";
        Console.WriteLine(updateCharacterSql);
        await conn.ExecuteAsync(updateCharacterSql, saveState, tx);
        //    await conn.ExecuteAsync(updatePlayerSql, saveState, tx);

        tx.Commit();
        return saveState;
    }

    public async Task<IEnumerable<PlayerCharacterDto>> GetCharactersByUserAsync(int userid)
    {
        const string sql = @"
        SELECT 
            pc.playercharacterid, pc.userid, pc.characterid, pc.gold, pc.activequest, pc.lastonline, pc.createdat,
            c.name, c.race, c.class, c.level, c.alignment, c.health, c.maxhealth, c.armorclass, c.strength, c.dexterity,
            c.constitution, c.intelligence, c.wisdom, c.charisma, c.hitdice, c.roomid, c.isalive, c.charactertype, c.createdat AS character_createdat
        FROM playercharacters pc
        INNER JOIN characters c ON pc.characterid = c.characterid
        WHERE pc.userid = @userid;
    ";

        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<PlayerCharacterDto>(sql, new { userid });
        return result;
    }

    public async Task<IEnumerable<RoomNpcDto>> GetNpcsInRoom(int roomId)
    {
        const string sql = @"
        SELECT c.characterid, c.name, c.race, c.class, c.level, c.alignment,
               c.health, c.maxhealth, c.armorclass, c.strength, c.dexterity,
               c.constitution, c.intelligence, c.wisdom, c.charisma, c.createdat,
               npc.npcid, npc.behaviortype, npc.ishostile, npc.respawntime,
               npc.loottableid, npc.roomid, npc.createdat as npc_createdat
        FROM nonplayercharacters npc
        INNER JOIN characters c ON npc.characterid = c.characterid
        WHERE npc.roomid = @roomId AND c.health > 0";

        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<Character, NonPlayerCharacters, RoomNpcDto>(
            sql,
            (character, npc) => new RoomNpcDto { Base = character, Instance = npc },
            new { roomId },
            splitOn: "npcid"
        );

        return result;
    }



}
