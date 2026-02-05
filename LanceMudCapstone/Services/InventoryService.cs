using LanceMudCapstone.DTOs;
using LanceMudCapstone.Enums;

namespace LanceMudCapstone.Services
{
    public class InventoryService
    {
        private readonly IItemRepository _itemRepo;
        private readonly IEquipRepository _equipRepo;
        private readonly CharacterService _characterService;

        public InventoryService(
            IItemRepository itemRepo,
            IEquipRepository equipRepo,
            CharacterService characterService)
        {
            _itemRepo = itemRepo;
            _equipRepo = equipRepo;
            _characterService = characterService;
        }

        public async Task<List<ItemDto>> GetInventory(int playerCharacterId)
        {
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
                MaxStack = r.MaxStack
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
                    MaxStack = r.MaxStack
                }
            }).ToList();
        }
    }
}
