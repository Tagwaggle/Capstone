using Dapper;

namespace LanceMudCapstone.Services
{
    public class RespawnService
    {
        private readonly DbHelper _db;
        public RespawnService(DbHelper db)
        {
            _db = db;
        }
        public async Task ProcessRespawns()
        {
            
            const string sql = @"
                SELECT npc.npcid, npc.characterid
                FROM nonplayercharacters npc
                JOIN characters c ON npc.characterid = c.characterid
                WHERE c.isalive = false
                    AND npc.respawnat IS NOT NULL
                    AND npc.respawnat <= NOW()";
            using var conn = await _db.CreateOpenConnectionAsync();
            var toRespawn = await conn.QueryAsync<(int NpcId, int CharacterId)>(sql);

            foreach (var npc in toRespawn) {
                Console.WriteLine("Respawining");
            await conn.ExecuteAsync(@"
                UPDATE characters
                SET isalive = true, health = maxhealth
                WHERE characterid = @CharacterId;", new { npc.CharacterId });

                await conn.ExecuteAsync(@"
                UPDATE nonplayercharacters
                SET respawnat = NULL
                WHERE npcid = @NpcId;", new { npc.NpcId });
                    }
        }
    }
}
