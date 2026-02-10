using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Enums;
using LanceMudCapstone.Models;
using LanceMudCapstone.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services
{
    public class CombatService : ICombatService
    {
        private readonly Random _rng = new();
        private readonly AbilityEngine _abilityEngine = new();
        private readonly DbHelper _db;
        private readonly InventoryService _inventoryService;
        private int[] junkIds = { 3, 4, 5, 6 };
        public CombatService(DbHelper db, InventoryService iServe)
        {
            _db = db;
            _inventoryService = iServe;
        }

        // Basic attack
        public async Task<CombatRoundResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender)
        {
            var log = new List<string>();
            int roll = _rng.Next(1, 21);
            int attackScore = roll + attacker.Strength;

            log.Add($"{attacker.Name} rolls {roll} + STR {attacker.Strength} = {attackScore}");

            if (attackScore >= defender.ArmorClass)
            {
                int damage = _rng.Next(1, 9); // 1d8
                defender.Health -= damage;
                if (defender.Health < 0) defender.Health = 0;

                log.Add($"{attacker.Name} hits {defender.Name} for {damage} damage!");
            }
            else
            {
                log.Add($"{attacker.Name} misses {defender.Name}.");
            }

            bool died = false;

            if (defender.Health <= 0 && defender.IsAlive)
            {
                defender.IsAlive = false;
                died = true;
                log.Add($"{defender.Name} has been defeated!");

                await HandleDeathAsync(defender);
            }

            return await Task.FromResult(new CombatRoundResult
            {
                Attacker = attacker,
                Defender = defender,
                Log = log,
                DamageDealt = 0, // basic attack doesn't track this yet
                TargetDied = died
            });
        }


        public async Task<CombatRoundResult> ResolveRoundAsync(PlayerCharacterDto player, PlayerCharacterDto mob)
        {
            var roundLog = new List<string>();

            var playerAttack = await AttackAsync(player, mob);
            roundLog.AddRange(playerAttack.Log);

            if (mob.IsAlive)
            {
                var mobAttack = await AttackAsync(mob, player);
                roundLog.AddRange(mobAttack.Log);
            }

            return new CombatRoundResult
            {
                Attacker = player,
                Defender = mob,
                Log = roundLog,
                DamageDealt = 0,
                TargetDied = !mob.IsAlive
            };
        }


        public async Task<CombatRoundResult> ResolveAbilityAsync(
            PlayerCharacterDto attacker,
            PlayerCharacterDto defender,
            AbilityDto ability)
        {
            var abilityResult = _abilityEngine.ExecuteAbility(attacker, defender, ability);

            return await Task.FromResult(new CombatRoundResult
            {
                Attacker = attacker,
                Defender = defender,
                Log = new List<string> { abilityResult.Message },
                DamageDealt = abilityResult.Damage,
                TargetDied = defender.Health <= 0
            });
        }


        public async Task<CombatRoundResult> CastSpellAsync(PlayerCharacterDto caster, PlayerCharacterDto target, Spell spell)
        {
            var log = new List<string>
            {
                $"{caster.Name} casts {spell.Name} on {target.Name}!"
            };

            return await Task.FromResult(new CombatRoundResult
            {
                Attacker = caster,
                Defender = target,
                Log = log
            });
        }


        public async Task<CombatRoundResult> UseItemAsync(PlayerCharacterDto user, Item item, PlayerCharacterDto? target = null)
        {
            var log = new List<string>
            {
                $"{user.Name} uses {item.Name}."
            };

            return await Task.FromResult(new CombatRoundResult
            {
                Attacker = user,
                Defender = target ?? user,
                Log = log
            });
        }

        public async Task<CombatRoundResult> RunAsync(PlayerCharacterDto runner, Room currentRoom)
        {
            var log = new List<string>
            {
                $"{runner.Name} attempts to flee from {currentRoom.Name}!"
            };

            return await Task.FromResult(new CombatRoundResult
            {
                Attacker = runner,
                Defender = runner,
                Log = log
            });
        }
        private async Task HandleDeathAsync(PlayerCharacterDto dead)
        {

            string testType = dead.CharacterType ?? string.Empty;
            if (testType == "pc")
            {
                await HandlePlayerDeath(dead);
            }
            else
            {
                await HanldeNPCDeath(dead);
            }
        }
        private async Task HandlePlayerDeath(PlayerCharacterDto deadCharacter)
        {
            var inventoryItems = await _inventoryService.GetInventory(deadCharacter.PlayerCharacterId);
            var equippedItems = await _inventoryService.GetEquipped(deadCharacter.PlayerCharacterId);

            var corpse = await CreateCorpseAsync(deadCharacter);

            foreach (var eq in equippedItems)
            {
                await _inventoryService.UnequipAsync(deadCharacter.PlayerCharacterId, eq.Slot);
            }

            await MoveInventoryToCorpseAsync(deadCharacter, corpse.ContainerId);

            string sql = @"
                UPDATE characters
                SET isalive = true, health = 1, roomid = 2
                WHERE characterid = @CharacterId;
";
            await _db.ExecuteAsync(sql, new { CharacterId = deadCharacter.CharacterId });
        }
        private async Task<Container> CreateCorpseAsync(PlayerCharacterDto deadCharacter)
        {
            int roomId = deadCharacter.RoomId ?? 20;

            string sql = @"
                INSERT INTO containers (name, roomid, ownernpcid, islootable, createdat)
                VALUES (@Name, @RoomId, @OwnerNPCId, @IsLootable, @CreatedAt)
                RETURNING containerid;
                        ";

            var corpse = new Container
            {
                Name = $"Corpse of {deadCharacter.Name}",
                RoomId = roomId,
                OwnerNPCId = 1,
                IsLootable = true,
                CreatedAt = DateTime.UtcNow
            };

            corpse.ContainerId = await _db.ExecuteScalarAsync<int>(sql, corpse);

            return corpse;
        }
        public async Task HanldeNPCDeath(PlayerCharacterDto deadCharacter)
        {
            var corpse = await CreateCorpseAsync(deadCharacter);
            var loot = await _inventoryService.GetLootForMobAsync(deadCharacter.CharacterId);
            using var conn = await _db.CreateOpenConnectionAsync();

            if (loot.Any())
            {
                foreach (var item in loot)
                {
                    await _inventoryService.MoveToContainerAsync(
                        deadCharacter.CharacterId,
                        item.ItemId,
                        corpse.ContainerId
                    );
                }
            }
            else
            {
                int junkId = junkIds[_rng.Next(junkIds.Length)];

                string junkSql = @"
            INSERT INTO roomitems (itemid, containerid, quantity, droppeddatetime)
            VALUES (@ItemId, @ContainerId, 1, NOW());
        ";

                await conn.ExecuteAsync(junkSql, new
                {
                    
                    ItemId = junkId,
                    ContainerId = corpse.ContainerId
                });
            }

            string sql = @"
        UPDATE characters
        SET isalive = false, health = 0
        WHERE characterid = @CharacterId;
    ";

            await _db.ExecuteAsync(sql, new { CharacterId = deadCharacter.CharacterId });
        }
        private async Task MoveInventoryToCorpseAsync(PlayerCharacterDto character, int containerId)
        {
            var inventory = await _inventoryService.GetInventory(character.PlayerCharacterId);
            int roomId = character.RoomId ?? 2;

            foreach (var item in inventory)
            {
                // Move into corpse container
                await _inventoryService.MoveToContainerAsync(
                    character.PlayerCharacterId,
                    item.ItemId,
                    containerId
                );

                // Remove from player inventory
                await _inventoryService.DeleteItemAsync(
                    character.PlayerCharacterId,
                    item.ItemId
                );
            }
        }

        public void ProcessCombatRounds()
        {
            // Future expansion
        }
    }
}
