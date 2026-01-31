namespace LanceMudCapstone.DTOs;

public class PlayerCharacterDto
{
    public int PlayerCharacterId { get; set; }
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Race { get; set; }
    public string? Class { get; set; }
    public int Level { get; set; }
    public string? Alignment { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int ArmorClass { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public string? HitDice { get; set; }
    public int RoomId { get; set; }
    public bool IsAlive { get; set; }
    public string CharacterType { get; set; } = string.Empty;
    public int Gold { get; set; }
    public int? ActiveQuest { get; set; }
    public DateTime? LastOnline { get; set; }
    public DateTime CreatedAt { get; set; }
}
