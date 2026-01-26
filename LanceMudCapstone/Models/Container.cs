namespace LanceMudCapstone.Models;

public class Container
{
    public int ContainerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public int? OwnerNPCId { get; set; }
    public bool IsLootable { get; set; }
    public DateTime CreatedAt { get; set; }

}
  