namespace LanceMudCapstone.Models;

public class EquippedItem
{
    public int EquippedItemId { get; set; }
    public int PlayerCharacterId { get; set; }
    public int ItemId { get; set; }
    public string Slot {  get; set; } = string.Empty;
    public DateTime EquippedAt { get; set; }

}
