namespace LanceMudCapstone.Models;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
    public int? Z { get; set; }
    public DateTime CreatedAt { get; set; }
}
