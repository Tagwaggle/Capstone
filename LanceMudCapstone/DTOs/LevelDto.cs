namespace LanceMudCapstone.DTOs;

public class LevelDto
{
   public PlayerCharacterDto Player { get; set; } = new();
   public string Message { get; set; } = string.Empty;
}
public class DamageDealtDto
{
    public int DamageeDelt { get; set; }
}