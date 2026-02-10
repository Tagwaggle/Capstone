namespace LanceMudCapstone.DTOs;

public class ContainerDto
{
    public int ContainerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public int? OwnerNpcId { get; set; }
    public bool IsLootable { get; set; }
    public DateTime CreatedAt { get; set; }
}
