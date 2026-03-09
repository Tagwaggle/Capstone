using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services
{
    public interface ICombatService
    {
        Task<CombatRoundResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender);

        Task<CombatRoundResult> ResolveRoundAsync(PlayerCharacterDto player, PlayerCharacterDto mob, AbilityDto? special = null);

        Task<CombatRoundResult> ResolveAbilityAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender, AbilityDto ability);

        Task<CombatRoundResult> UseItemAsync(PlayerCharacterDto user, Item item, PlayerCharacterDto? target = null);

        Task<CombatRoundResult> RunAsync(PlayerCharacterDto runner, Room currentRoom);

        void ProcessCombatRounds()
        {
            // This method would contain the logic to process ongoing combat rounds, such as applying damage over time effects, checking for deaths, and updating combat states.
        }
    }
}
