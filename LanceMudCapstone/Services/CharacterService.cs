using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services;

public class CharacterService
{
    private readonly DbHelper _db;

    public CharacterService(DbHelper db)
    {
        _db = db;
    }

    public Task<int> CreateCharacterAsync(CreateCharacterDto dto)
        => Task.FromResult(0);

    public Task<bool> UpdateCharacterAsync(UpdateCharacterDto dto)
        => Task.FromResult(true);

    public Task<CharacterDto?> GetCharacterByIdAsync(int id)
        => Task.FromResult<CharacterDto?>(null);

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
        WHERE npc.roomid = @roomId";

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
