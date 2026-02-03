using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

public interface ICombatService
{
    Task<CombatResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender);
    Task<CombatRoundResult> ResolveRoundAsync(PlayerCharacterDto player, PlayerCharacterDto mob);
    Task<CombatResult> CastSpellAsync(PlayerCharacterDto caster, PlayerCharacterDto target, Spell spell);
    Task<CombatResult> UseItemAsync(PlayerCharacterDto user, Item item, PlayerCharacterDto? target = null);
    Task<CombatResult> RunAsync(PlayerCharacterDto runner, Room currentRoom);
}
