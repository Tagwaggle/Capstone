namespace LanceMudCapstone.Models;

public class PlayerCharacter
{
    public int PlayerCharacterId { get; set; }
    public int CharacterId { get; set; }
    public int UserId { get; set; }
    public int Gold { get; set; }
    public int? ActiveQuest { get; set; }
    public DateTime? LastOnline { get; set; }
    public DateTime CreatedAt { get; set; }
}
