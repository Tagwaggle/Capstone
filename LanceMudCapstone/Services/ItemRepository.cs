using Dapper;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using LanceMudCapstone.DTOs;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services
{
    public class ItemRepository : IItemRepository
    {

        private readonly DbHelper _db;
        public ItemRepository(DbHelper db)
        {
            _db = db;
        }

        public async Task<Item?> GetItemForPlayerAsync(int playerId, int itemId)
        {
            const string sql = @"
            SELECT i.*
            FROM items i
            INNER JOIN playerinventory pi ON pi.itemid = i.itemid
            WHERE pi.playercharacterid = @playerId AND i.itemid = @itemId;";

            await using var conn = await _db.CreateOpenConnectionAsync();
            return await conn.QueryFirstOrDefaultAsync<Item>(sql, new { playerId, itemId });

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

    }
}
