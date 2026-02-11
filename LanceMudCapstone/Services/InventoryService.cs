using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Enums;
using LanceMudCapstone.Models;


namespace LanceMudCapstone.Services
{
    public class InventoryService
    {
        private readonly IItemRepository _itemRepo;
        private readonly IEquipRepository _equipRepo;
        private readonly CharacterService _characterService;
        private readonly DbHelper _db;

        public InventoryService(
            IItemRepository itemRepo,
            IEquipRepository equipRepo,
            CharacterService characterService,
            DbHelper db)
        {
            _itemRepo = itemRepo;
            _equipRepo = equipRepo;
            _characterService = characterService;
            _db = db;
        }

        public async Task<List<ItemDto>> GetInventory(int playerCharacterId)
        {
            Console.WriteLine("InventoryService.GetInventory called");

            var rows = await _itemRepo.GetInventory(playerCharacterId);

            return rows.Select(r => new ItemDto
            {
                ItemId = r.ItemId,
                Name = r.Name,
                Description = r.Description,
                ItemType = r.ItemType,
                Slot = string.IsNullOrEmpty(r.Slot) ? null : Enum.Parse<EquipSlot>(r.Slot),
                Value = r.Value,
                EffectJson = r.EffectJson,
                Stackable = r.Stackable,
                Quantity = r.Quantity,
                MaxStack = r.MaxStack,
                ItemCategory = r.ItemCategory
            }).ToList();
        }

        public async Task<List<EquippedItemDto>> GetEquipped(int playerCharacterId)
        {
            var rows = await _equipRepo.GetEquipped(playerCharacterId);

            return rows.Select(r => new EquippedItemDto
            {
                Slot = Enum.Parse<EquipSlot>(r.Slot),
                Item = new ItemDto
                {
                    ItemId = r.ItemId,
                    Name = r.Name,
                    Description = r.Description,
                    ItemType = r.ItemType,
                    Slot = string.IsNullOrEmpty(r.ItemSlot) ? null : Enum.Parse<EquipSlot>(r.ItemSlot),
                    Value = r.Value,
                    EffectJson = r.EffectJson,
                    Stackable = r.Stackable,
                    Quantity = 1,
                    MaxStack = r.MaxStack,

                }
            }).ToList();
        }
        public async Task UnequipAsync(int playerCharacterId, EquipSlot slot)
        {
            var equipped = await _equipRepo.GetEquippedItemAsync(playerCharacterId, slot);
            if (equipped == null) return;

            await _equipRepo.UnequipAsync(equipped.EquippedItemId);
            await _itemRepo.AddToInventory(playerCharacterId, equipped.ItemId, 1);

            await _characterService.RecalculateStats(playerCharacterId);
        }
        public async Task EquipAsync(int playerCharacterId, int itemId, EquipSlot slot)
        {

            var existing = await _equipRepo.GetEquippedItemAsync(playerCharacterId, slot);
            if (existing != null)
            {
                await _equipRepo.UnequipAsync(existing.EquippedItemId);
                await _itemRepo.AddToInventory(playerCharacterId, existing.ItemId, 1);
            }

            await _itemRepo.RemoveFromInventory(playerCharacterId, itemId, 1);

            await _equipRepo.EquipAsync(playerCharacterId, itemId, slot);

            await _characterService.RecalculateStats(playerCharacterId);
        }
        public async Task<string> UseAsync(int playerCharacterId, int itemId)
        {
            var item = await _itemRepo.GetItemForPlayerAsync(playerCharacterId, itemId);
            if (item == null)
                return "You don't have that item.";

            if (item.ItemCategory != ItemCategory.Consumable)
                return "You can't use that item.";

            var effect = ItemEffectParser.Parse(item.EffectJson);
            if (effect == null)
                return "Nothing happens.";

            var resultMessage = await ApplyEffect(playerCharacterId, effect);

            await _itemRepo.RemoveFromInventory(playerCharacterId, itemId, 1);

            return resultMessage;
        }
        public async Task<string> ApplyEffect(int playerCharacterId, ItemEffect effect)
        {
            var character = await _characterService.GetCharacterByIdAsync(playerCharacterId);
            if (character == null)
                return "Character not found.";

            string message = "";

            if (effect.HealthDelta.HasValue)
            {
                character.Health += effect.HealthDelta.Value;

                if (character.Health > character.MaxHealth)
                    character.Health = character.MaxHealth;

                if (character.Health < 0)
                    character.Health = 1;

                message += $"Your health {(effect.HealthDelta > 0 ? "increases" : "decreases")} by {Math.Abs(effect.HealthDelta.Value)}. ";
            }

            var updateDto = new UpdateCharacterDto
            {
                CharacterId = character.CharacterId,
                Health = character.Health
            };

            await _characterService.UpdateCharacterAsync(updateDto);

            return message.Trim();
        }

        public async Task MoveToContainerAsync(int playerCharacterId, int itemId, int containerId)
        {
            using var conn = await _db.CreateOpenConnectionAsync();

            string sql = @"
        INSERT INTO roomitems (itemid, containerid, quantity, droppeddatetime)
        VALUES (@ItemId, @ContainerId, 1, NOW());
    ";

            await conn.ExecuteAsync(sql, new
            {
                ItemId = itemId,
                ContainerId = containerId
            });
        }

        public async Task DeleteItemAsync(int playerCharacterId, int itemId)
        {
            using var conn = await _db.CreateOpenConnectionAsync();

            string sql = @"
                DELETE FROM playerinventory
                WHERE ctid IN (
                SELECT ctid FROM playerinventory
                WHERE playercharacterid = @PlayerId AND itemid = @ItemId
                LIMIT 1);";
            await conn.ExecuteAsync(sql, new
            {
                PlayerId = playerCharacterId,
                ItemId = itemId
            });
        }
        public async Task<List<LootItemDto>> GetLootForMobAsync(int npcId)
        {
            using var conn = await _db.CreateOpenConnectionAsync();

            string sql = @"
                    SELECT 
                        lti.itemid, lti.quantity, i.name, i.description, i.itemtype, i.itemcategory
                    FROM nonplayercharacters npc
                    INNER JOIN loottables lt ON lt.loottableid = npc.loottableid
                    INNER JOIN loottableitems lti ON lti.loottableid = lt.loottableid
                    INNER JOIN items i ON i.itemid = lti.itemid
                    WHERE npc.npcid = @NpcId;
                ";

            var results = await conn.QueryAsync<LootItemDto>(sql, new { NpcId = npcId });
            return results.ToList();
        }
        public async Task<ItemDtoRaw?> GetItemById(int itemId)
        {
            return await _itemRepo.GetItemById(itemId);
        }

    }
}
