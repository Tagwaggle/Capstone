namespace LanceMudCapstone.Models;

public class RoomExit
{
    public int ExitId { get; set; }
    public int FromRoomId { get; set; }
    public int ToRoomId { get; set; }
    public string Direction { get; set; } = string.Empty;

}
