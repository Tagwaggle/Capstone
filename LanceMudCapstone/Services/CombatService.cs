using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services
{
    public class CombatService : ICombatService
    {
        private readonly Random _rng = new();

        // Atomic attack: one attacker vs one defender
        public async Task<CombatResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender)
        {
            var log = new List<string>();
            int roll = _rng.Next(1, 21);
            int attackScore = roll + attacker.Strength;

            log.Add($"{attacker.Name} rolls {roll} + STR {attacker.Strength} = {attackScore}");

            if (attackScore >= defender.ArmorClass)
            {
                int damage = _rng.Next(1, 9); // 1d8 damage
                defender.Health -= damage;
                if (defender.Health < 0) defender.Health = 0;

                log.Add($"{attacker.Name} hits {defender.Name} for {damage} damage!");
            }
            else
            {
                log.Add($"{attacker.Name} misses {defender.Name}.");
            }

            if (defender.Health <= 0 && defender.IsAlive)
            {
                defender.IsAlive = false;
                log.Add($"{defender.Name} has been defeated!");
            }

            return await Task.FromResult(new CombatResult
            {
                Attacker = attacker,
                Defender = defender,
                Log = log
            });
        }


        // Full round: player attacks mob, mob retaliates if alive
        public async Task<CombatRoundResult> ResolveRoundAsync(PlayerCharacterDto player, PlayerCharacterDto mob)
        {
            var roundLog = new List<string>();

            Console.WriteLine($"Player HP before: {player.Health}");
            Console.WriteLine($"Skeleton HP before: {mob.Health}");
            Console.WriteLine($"Player attacks Skeleton (AC {mob.ArmorClass})");
            Console.WriteLine($"Skeleton attacks Player (AC {player.ArmorClass})");

            var playerAttack = await AttackAsync(player, mob);
            roundLog.AddRange(playerAttack.Log);

            if (mob.IsAlive)
            {
                var mobAttack = await AttackAsync(mob, player);
                roundLog.AddRange(mobAttack.Log);
            }

            return new CombatRoundResult
            {
                Player = player,
                Mob = mob,
                Log = roundLog
            };
        }

        // Spell casting
        public async Task<CombatResult> CastSpellAsync(PlayerCharacterDto caster, PlayerCharacterDto target, Spell spell)
        {
            var log = new List<string>
            {
                $"{caster.Name} casts {spell.Name} on {target.Name}!"
            };

            // TODO: implement spell effects (damage, healing, buffs)
            return await Task.FromResult(new CombatResult
            {
                Attacker = caster,
                Defender = target,
                Log = log
            });
        }

        // Item usage
        public async Task<CombatResult> UseItemAsync(PlayerCharacterDto user, Item item, PlayerCharacterDto? target = null)
        {
            var log = new List<string>
            {
                $"{user.Name} uses {item.Name}."
            };

            // TODO: implement item effects
            return await Task.FromResult(new CombatResult
            {
                Attacker = user,
                Defender = target ?? user,
                Log = log
            });
        }

        // Attempt to flee
        public async Task<CombatResult> RunAsync(PlayerCharacterDto runner, Room currentRoom)
        {
            var log = new List<string>
            {
                $"{runner.Name} attempts to flee from {currentRoom.Name}!"
            };

            // TODO: implement escape chance logic
            return await Task.FromResult(new CombatResult
            {
                Attacker = runner,
                Defender = runner, // no defender in a flee action
                Log = log
            });
        }
        public void ProcessCombatRounds()
        {
            // Placeholder for future combat round processing logic
        }
    }
}
