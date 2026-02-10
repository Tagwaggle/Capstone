namespace LanceMudCapstone.DTOs;


public class CombatRoundResult
{
    public List<string> Log { get; set; } = new();
    public int DamageDealt { get; set; }
    public bool TargetDied { get; set; }
    public PlayerCharacterDto Attacker { get; set; } = default!;
    public PlayerCharacterDto Defender { get; set; } = default!;
}
