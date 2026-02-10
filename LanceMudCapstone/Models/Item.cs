using LanceMudCapstone.Enums;

namespace LanceMudCapstone.Models;

public class Item
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description {  get; set; }
    public string ItemType { get; set; } = string.Empty;
    public EquipSlot? Slot {  get; set; }
    public int Value { get; set; }
    public string? effect {  get; set; }
    public bool Stackable { get; set; }
    public int? MaxStack { get; set; }
    public string ItemCategory { get; set; } = "Misc";
    public DateTime CreatedAt { get; set; }

}