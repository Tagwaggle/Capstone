namespace LanceMudCapstone.DTOs;

public class AbilityDto
{
    public string Name { get; set; } = string.Empty;
    public int LevelRequirement { get; set; }
    public string DamageFormula { get; set; } = string.Empty;
    public int CooldownSeconds { get; set; }
    public int? StaminaCost { get; set; }
    public int? ManaCost { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
