using LanceMudCapstone.Enums;

namespace LanceMudCapstone.Models;

public class EquippedItem
{
    public int EquippedItemId { get; set; }
    public int PlayerCharacterId { get; set; }
    public int ItemId { get; set; }
    public EquipSlot Slot { get; set; }
    public DateTime EquippedAt { get; set; }

}
