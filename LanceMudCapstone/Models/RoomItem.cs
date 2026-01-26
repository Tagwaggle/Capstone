namespace LanceMudCapstone.Models;

public class RoomItem
{
    public int RoomItemId { get; set; }
    public int? ContainerId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public DateTime DroppedDateTime { get; set; }


}
