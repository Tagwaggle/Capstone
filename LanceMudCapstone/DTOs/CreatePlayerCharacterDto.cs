namespace LanceMudCapstone.DTOs;

public class CreatePlayerCharacterDto
{
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int Gold { get; set; } = 0;
    public int? ActiveQuest { get; set; }
    public DateTime? LastOnline { get; set; }
}
