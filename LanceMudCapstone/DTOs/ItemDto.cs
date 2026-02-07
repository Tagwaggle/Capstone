using LanceMudCapstone.Enums;

namespace LanceMudCapstone.DTOs
{
    public class ItemDto
    {
        public int ItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ItemType { get; set; } = string.Empty;

        public EquipSlot? Slot { get; set; }

        public int Value { get; set; }

        public bool Stackable { get; set; }
        public int Quantity { get; set; } = 1;
        public int? MaxStack { get; set; }

        public string? EffectJson { get; set; }

        public bool IsEquipped { get; set; }
        public ItemCategory ItemCategory { get; set; }
    }
}
