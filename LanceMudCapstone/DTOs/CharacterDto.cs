namespace LanceMudCapstone.DTOs;

public class UserEmailDto
{
    public string Email { get; set; } = string.Empty;
}
public class PasswordDto
{
    public string Password { get; set; } = string.Empty;
}
public class CharacterDto
{
    public int? UserId { get; set; }
    public int CharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ALignment { get; set; } = string.Empty;

    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int ArmorClass { get; set; }

    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public string HitDice { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public bool isAlive { get; set; }
    public string CharacterType { get; set; } = string.Empty;
}
