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
        public async Task AutoLogoutStaleCharacter()
        {
            const string sql = @"
            UPDATE playercharacters
            SET isonline = false
            WHERE isonline = true
                AND lastonline < NOW() - INTERVAL '5 minutes';
        ";
            using var conn = await _db.CreateOpenConnectionAsync();
            await conn.ExecuteAsync(sql);

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
        public async Task TickHeal()
        {
            const string onlinesql = @"
            UPDATE characters c
            SET c.health = LEAST(c.maxhealth, c.health + 5)
            FROM playercharacters pc
            WHERE pc.characterid = c.characterid
            AND pc.isonline = true;";
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(onlinesql);
            const string offlinesql = @"
            UPDATE characters c
            SET c.health = LEAST(c.maxhealth, c.health + 2)
            FROM playercharacters pc
            WHERE pc.characterid = c.characterid
            AND pc.isonline = false;";
            await conn.ExecuteAsync(offlinesql);
            const string npcsql = @"
            UPDATE characters c
            SET c.health = LEAST(c.maxhealth, c.health + 3)
            FROM nonplayercharacters npc
            WHERE npc.characterid = c.characterid
            AND c.isalive = true;";
            await conn.ExecuteAsync(npcsql);
        }
    }
}
