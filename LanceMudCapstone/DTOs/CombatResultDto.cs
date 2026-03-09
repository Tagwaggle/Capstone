namespace LanceMudCapstone.DTOs;

public class CombatResultDto
{
    public List<string> Log { get; set; } = new();
    public int DamageDealt { get; set; }
    public bool TargetDied { get; set; }
    //    public List<string> Log { get; set; } new();
}
