namespace LanceMudCapstone.Models;

public class Ability
{
    public int AbilityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? ClassRestriction { get; set; }
    public int LevelRequirement { get; set; }
    public int ManaCost { get; set; }
    public int StaminaCost { get; set; }
    public int CoolDownSeconds { get; set; }
    public string? Description { get; set; }
    public string? damageformula { get; set; }
    public string? effectjson { get; set; }

}
