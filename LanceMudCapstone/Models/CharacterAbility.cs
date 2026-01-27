namespace LanceMudCapstone.Models;

public class CharacterAbility
{
    public int CharacterAbilityId { get; set; }
    public int CharacterId { get; set; }
    public int AbilityId { get; set; }
    public bool IsLearned { get; set; }
    public int? HotBarSlot { get; set; }
}
