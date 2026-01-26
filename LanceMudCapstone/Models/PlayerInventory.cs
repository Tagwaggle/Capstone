namespace LanceMudCapstone.Models;

public class PlayerInventory
{
    public int PlayerInventoryId { get; set; }
    public int PlayerCharacterId {  get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

}
