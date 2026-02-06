using LanceMudCapstone.Enums;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using Dapper;

namespace LanceMudCapstone.Services
{
    public class EquipRepository : IEquipRepository
    {
        private readonly DbHelper _db;

        public EquipRepository(DbHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<EquippedItemRaw>> GetEquipped(int playerCharacterId)
        {
            const string sql = @"
                SELECT 
                    ei.equippeditemid,
                    ei.slot,
                    i.itemid,
                    i.name,
                    i.description,
                    i.itemtype,
                    i.slot AS itemslot,
                    i.value,
                    i.effect AS EffectJson,
                    i.stackable,
                    i.maxstack
                FROM equippeditems ei
                INNER JOIN items i ON i.itemid = ei.itemid
                WHERE ei.playercharacterid = @playerCharacterId;
            ";

            using var conn = await _db.CreateOpenConnectionAsync();
            return await conn.QueryAsync<EquippedItemRaw>(sql, new { playerCharacterId });
        }

        public async Task<EquippedItem?> GetEquippedItemAsync(int playerId, EquipSlot slot)
        {
            Console.WriteLine("InventoryService.GetEquipped called");

            const string sql = @"
                SELECT *
                FROM equippeditems
                WHERE playercharacterid = @playerId
                  AND slot = @slot;
            ";

            using var conn = await _db.CreateOpenConnectionAsync();
            return await conn.QueryFirstOrDefaultAsync<EquippedItem>(sql, new { playerId, slot = slot.ToString() });
        }

        public async Task EquipAsync(int playerId, int itemId, EquipSlot slot)
        {
            const string sql = @"
                INSERT INTO equippeditems (playercharacterid, itemid, slot, equippedat)
                VALUES (@playerId, @itemId, @slot, NOW());
            ";

            using var conn = await _db.CreateOpenConnectionAsync();
            await conn.ExecuteAsync(sql, new { playerId, itemId, slot = slot.ToString() });
        }


        public async Task UnequipAsync(int equippedItemId)
        {
            const string sql = @"DELETE FROM equippeditems WHERE equippeditemid = @equippedItemId;";

            using var conn = await _db.CreateOpenConnectionAsync();
            await conn.ExecuteAsync(sql, new { equippedItemId });
        }
    }
}
