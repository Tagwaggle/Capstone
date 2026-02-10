namespace LanceMudCapstone.DTOs;

public class LootItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ItemType { get; set; } = "";
    public string ItemCategory { get; set; } = "";
}
