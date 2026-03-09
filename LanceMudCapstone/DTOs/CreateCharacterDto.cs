namespace LanceMudCapstone.DTOs;

public class CreateCharacterDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;
    public int RoomId { get; set; }
}
