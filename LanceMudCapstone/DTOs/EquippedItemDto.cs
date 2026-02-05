using LanceMudCapstone.Enums;

namespace LanceMudCapstone.DTOs
{
    public class EquippedItemDto
    {
        public EquipSlot Slot { get; set; }
        public ItemDto Item { get; set; } = new();
    }
}
