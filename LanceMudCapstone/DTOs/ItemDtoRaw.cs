using LanceMudCapstone.Enums;

namespace LanceMudCapstone.DTOs
{
    public class ItemDtoRaw
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string ItemType { get; set; } = "";
        public string? Slot { get; set; }
        public int Value { get; set; }
        public string? EffectJson { get; set; }
        public bool Stackable { get; set; }
        public int Quantity { get; set; }
        public int? MaxStack { get; set; }
        public ItemCategory ItemCategory { get; set; }

    }
}
