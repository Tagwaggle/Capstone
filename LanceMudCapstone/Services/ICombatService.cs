using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services
{
    public interface ICombatService
    {
        Task<CombatRoundResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender);

        Task<CombatRoundResult> ResolveRoundAsync(PlayerCharacterDto player, PlayerCharacterDto mob);

        Task<CombatRoundResult> ResolveAbilityAsync(
            PlayerCharacterDto attacker,
            PlayerCharacterDto defender,
            AbilityDto ability);

        //Task<CombatRoundResult> CastSpellAsync(PlayerCharacterDto caster, PlayerCharacterDto target, Spell spell);

        Task<CombatRoundResult> UseItemAsync(PlayerCharacterDto user, Item item, PlayerCharacterDto? target = null);

        Task<CombatRoundResult> RunAsync(PlayerCharacterDto runner, Room currentRoom);

        void ProcessCombatRounds()
        {
            // This method would contain the logic to process ongoing combat rounds, such as applying damage over time effects, checking for deaths, and updating combat states.
        }
    }
}
