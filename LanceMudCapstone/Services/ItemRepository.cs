using Dapper;
using LanceMudCapstone.DTOs;

namespace LanceMudCapstone.Services
{
    public class ItemRepository : IItemRepository
    {

        private readonly DbHelper _db;
        public ItemRepository(DbHelper db)
        {
            _db = db;
        }
        public async Task<ItemDtoRaw?> GetItemById(int itemId)
        {
            using var conn = await _db.CreateOpenConnectionAsync();

            string sql = "SELECT itemid, name, description, itemtype, slot, value, effect AS EffectJson, stackable, maxstack, itemcategory FROM items WHERE itemid = @itemId";
            return await conn.QuerySingleOrDefaultAsync<ItemDtoRaw>(sql, new { itemId });
        }
        public async Task<ItemDtoRaw?> GetItemForPlayerAsync(int playerCharacterId, int itemId)
        {
            const string sql = @"
        SELECT 
            i.itemid,
            i.name,
            i.description,
            i.itemtype,
            i.slot,
            i.value,
            i.effect AS EffectJson,
            i.stackable,
            pi.quantity,
            i.maxstack,
            i.itemcategory
        FROM items i
        INNER JOIN playerinventory pi ON pi.itemid = i.itemid
        WHERE pi.playercharacterid = @playerCharacterId 
          AND i.itemid = @itemId;
    ";

            await using var conn = await _db.CreateOpenConnectionAsync();
            return await conn.QueryFirstOrDefaultAsync<ItemDtoRaw>(
                sql,
                new { playerCharacterId, itemId }
            );
        }

        public async Task<IEnumerable<ItemDtoRaw>> GetInventory(int playerCharacterId)
        {
            const string sql = @"
        SELECT 
            i.itemid,
            i.name,
            i.description,
            i.itemtype,
            i.slot,
            i.value,
            i.effect AS EffectJson,
            i.stackable,
            pi.quantity,
            i.maxstack
        FROM playerinventory pi
        INNER JOIN items i ON i.itemid = pi.itemid
        WHERE pi.playercharacterid = @playerCharacterId
        ORDER BY i.name;
    ";

            using var conn = await _db.CreateOpenConnectionAsync();
            return await conn.QueryAsync<ItemDtoRaw>(sql, new { playerCharacterId });
        }
        public async Task AddToInventory(int playerCharacterId, int itemId, int quantity)
        {
            const string sql = @"
                INSERT INTO playerinventory (playercharacterid, itemid, quantity)
                VALUES (@playerCharacterId, @itemId, @quantity)
                ON CONFLICT (playercharacterid, itemid)
                DO UPDATE SET quantity = playerinventory.quantity + @quantity;";

            using var conn = await _db.CreateOpenConnectionAsync();
            await conn.ExecuteAsync(sql, new { playerCharacterId, itemId, quantity });
        }
        public async Task RemoveFromInventory(int playerCharacterId, int itemId, int quantity)
        {
            const string sql = @"
                    UPDATE playerinventory
                    SET quantity = quantity - @quantity
                    WHERE playercharacterid = @playerCharacterId
                      AND itemid = @itemId;

                    DELETE FROM playerinventory
                    WHERE playercharacterid = @playerCharacterId
                      AND itemid = @itemId
                      AND quantity <= 0;
                ";

            using var conn = await _db.CreateOpenConnectionAsync();
            await conn.ExecuteAsync(sql, new { playerCharacterId, itemId, quantity });
        }




        public async Task<List<LootItemDto>> GetLootForMobAsync(int npcId)
        {
            using var conn = await _db.CreateOpenConnectionAsync();
            string sql = @"
                    SELECT lt.itemid, lt.quantity, i.name, i.description
                    FROM loottables lt
                    INNER JOIN items i ON i.itemid = lt.itemid
                    WHERE lt.npcid = @NpcId;";

            var result = await conn.QueryAsync<LootItemDto>(sql, new { NpcId = npcId });
            foreach (var loot in result)
            {

            }
            return result.ToList();
        }
    }
}