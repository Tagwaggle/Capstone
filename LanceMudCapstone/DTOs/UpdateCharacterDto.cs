namespace LanceMudCapstone.DTOs;

public class UpdateCharacterDto
{
    public int CharacterId { get; set; }

    public int? Health { get; set; }
    public int? MaxHealth { get; set; }
    public int? ArmorClass { get; set; }

    public int? Strength { get; set; }
    public int? Dexterity { get; set; }
    public int? Constitution { get; set; }
    public int? Intelligence { get; set; }
    public int? Wisdom { get; set; }
    public int? Charisma { get; set; }

    public int? RoomId { get; set; }
    public bool? IsAlive { get; set; }
}
