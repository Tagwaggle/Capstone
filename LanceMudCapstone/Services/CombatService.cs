    using Dapper;
    using LanceMudCapstone.DTOs;
    using LanceMudCapstone.Enums;
    using LanceMudCapstone.Models;
    using LanceMudCapstone.Services;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.IdentityModel.Tokens;
    using System;
using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    namespace LanceMudCapstone.Services
    {
        public class CombatService : ICombatService
        {
            private readonly Random _rng = new();
            private readonly AbilityEngine _abilityEngine = new();
            private readonly DbHelper _db;
            private readonly InventoryService _inventoryService;
            private readonly CharacterService _characterService;
            private readonly SessionState SessionState;
            private int[] junkIds = { 3, 4, 5, 6 };
            public CombatService(DbHelper db, InventoryService iServe, CharacterService characterService, SessionState sessionState)
            {
                _db = db;
                _inventoryService = iServe;
                _characterService = characterService;
                SessionState = sessionState;
            }

        private int StatBonus(int stat)
        {
            int bonus;
            switch (stat)
            {
                case 1:
                case 2:
                case 3:
                    bonus = 0;
                    break;
                case 4:
                case 5:
                case 6:
                    bonus = 1;
                    break;
                case 7:
                case 8:
                case 9:
                    bonus = 3;
                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                    bonus = 4;
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                    bonus = 5;
                    break;
                default:
                    bonus = 7;
                    break;

            }
            return bonus;
        }
        private async Task<DamageDealtDto> DamageDelt(PlayerCharacterDto attacker, PlayerCharacterDto defender)
        {
            int _DamageDelt = 1;
            if (attacker.CharacterType == "pc")
            {
                var equipped = await _inventoryService.GetEquipped(attacker.PlayerCharacterId!.Value);

                var mainHand = equipped.FirstOrDefault(e => e.Slot == EquipSlot.MainHand);

                int weaponBonus = 0;
                int strBonus = StatBonus(attacker.Strength);
                int weaponDamage = 0;

                if (mainHand != null)
                {
                    var weapon = await _inventoryService.GetItemById(mainHand.Item.ItemId);

                    if (!string.IsNullOrEmpty(weapon.EffectJson))
                    {
                        var effect = JsonSerializer.Deserialize<WeaponEffect>(weapon.EffectJson);
                        weaponBonus = effect?.DamageBonus ?? 0;
                    }

                    weaponDamage = _rng.Next(2, 12);
                }
                else
                {
                    weaponDamage = _rng.Next(1, 5);
                }

                _DamageDelt = weaponDamage + weaponBonus + strBonus;
            }
            else _DamageDelt = _rng.Next(1, 5) + StatBonus(attacker.Strength);

            return new DamageDealtDto
                {
                    DamageeDelt = _DamageDelt
                };
        }
        // Basic attack
        public async Task<CombatRoundResult> AttackAsync(PlayerCharacterDto attacker, PlayerCharacterDto defender)
        {
            var log = new List<string>();
            int roll = _rng.Next(1, 21);
            var attack = await DamageDelt(attacker, defender);
            int DamageeDelt = attack.DamageeDelt;
            int attackScore = roll + StatBonus(attacker.Dexterity);

            log.Add($"{attacker.Name} rolls {roll} + STR {attacker.Strength} = {attackScore}");

            if (attackScore >= defender.ArmorClass)
            {
                int damage = DamageeDelt;
                defender.Health -= damage;
                if (defender.Health < 0) defender.Health = 0;

                string temp = $"{attacker.Name} hits {defender.Name} for {damage} damage!";
                log.Add(temp);
                await SessionState.PushEvent(temp);
                await SessionState.NotifyStateChange();
            }
            else
            {
                string temp = $"{attacker.Name} misses {defender.Name}.";
                log.Add(temp);
                await SessionState.PushEvent(temp);
                await SessionState.NotifyStateChange();
            }

            bool died = false;

            if (defender.Health <= 0 && defender.IsAlive)
            {
                defender.IsAlive = false;
                died = true;
                log.Add($"{defender.Name} has been defeated!");

                await SessionState.PushEvent($"{defender.Name} has been defeated by {attacker.Name}!");
                await SessionState.NotifyStateChange();

                if (attacker.CharacterType == "pc" && defender.CharacterType != "pc")
                    await GainXP(attacker, defender);

                await HandleDeathAsync(defender);
            }

            return new CombatRoundResult
            {
                Attacker = attacker,
                Defender = defender,
                Log = log,
                DamageDealt = DamageeDelt,
                TargetDied = died
            };
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
                if (defender.Health <= 0 && !defender.IsAlive)
                {
                defender.IsAlive = false;

                await SessionState.PushEvent($"{defender.Name} has been defeated by {attacker.Name}'s {ability.Name}!");
                await SessionState.NotifyStateChange();

                if (attacker.CharacterType == "pc" && defender.CharacterType != "pc") await GainXP(attacker, defender);
                await HandleDeathAsync(defender);
            }
            return await Task.FromResult(new CombatRoundResult
                {
                    Attacker = attacker,
                    Defender = defender,
                    Log = new List<string> { abilityResult.Message },
                    DamageDealt = abilityResult.Damage,
                    TargetDied = defender.Health <= 0
                });
            }


        /*    public async Task<CombatRoundResult> CastSpellAsync(PlayerCharacterDto caster, PlayerCharacterDto target, Spell spell)
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
        */

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
                    var room = SessionState.CurrentRoom;
                    if (room != null && SessionState.CurrentRoomMobs != null)
                    {
                        SessionState.CurrentRoomMobs.RemoveAll(m => m.Instance.NpcId == dead.NpcId);
                        await SessionState.PushEvent($"The {dead.Name} has been defeated!");
                    }

                }
            
            await SessionState.NotifyStateChange();
            }
        private async Task HandlePlayerDeath(PlayerCharacterDto deadCharacter)
            {
                var inventoryItems = await _inventoryService.GetInventory(deadCharacter.PlayerCharacterId!.Value);
                var equippedItems = await _inventoryService.GetEquipped(deadCharacter.PlayerCharacterId!.Value);

                var corpse = await CreateCorpseAsync(deadCharacter);

                foreach (var eq in equippedItems)
                {
                    await _inventoryService.UnequipAsync(deadCharacter.PlayerCharacterId!.Value, eq.Slot);
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
            string respawnSql = @"
                        UPDATE nonplayercharacters
                        SET lastkilled = NOW(),
                            respawnat = NOW() + (respawntime || ' seconds')::interval
                        WHERE npcid = @NpcId;
                    ";

            await _db.ExecuteAsync(respawnSql, new { NpcId = deadCharacter.NpcId });
            }
            private async Task MoveInventoryToCorpseAsync(PlayerCharacterDto character, int containerId)
            {
                var inventory = await _inventoryService.GetInventory(character.PlayerCharacterId!.Value);
                int roomId = character.RoomId ?? 2;

                foreach (var item in inventory)
                {
                    // Move into corpse container
                    await _inventoryService.MoveToContainerAsync(
                        character.PlayerCharacterId!.Value,
                        item.ItemId,
                        containerId
                    );

                    // Remove from player inventory
                    await _inventoryService.DeleteItemAsync(
                        character.PlayerCharacterId!.Value,
                        item.ItemId
                    );
                }
            }
            public async Task<LevelDto> GainLevel(PlayerCharacterDto player)
            {

                player.Level++;
                int Health = _rng.Next(2, 10);
                int Mana = _rng.Next(1, 4);
                int Stamina = _rng.Next(1, 4);
                string message = "Congratulations! You've reached level " + player.Level + ", You gain ";
                player.MaxHealth += Health;
                message += Health + " health";
                if (player.MaxMana != 0)
                {
                    message += ", " + Mana + " mana ";
                    player.MaxMana += Mana;
                }
                if (player.MaxStamina != 0)
                {
                    message += ", " + Stamina + " stamina ";
                    player.MaxStamina += Stamina;
                }
                message += "!";
                player.Health = player.MaxHealth;
                player.Mana = player.MaxMana;
                player.Stamina = player.MaxStamina;
               
                player.Xp = 0;
                player.XpNeeded = player.Level * 100;

                return new LevelDto
                {
                    Message = message,
                    Player = player
                };
            }
            public async Task GainXP(PlayerCharacterDto player, PlayerCharacterDto dead)
            {
                int XpGained = dead.Xp;
                if (XpGained == 0) XpGained = 25;
                bool _level = false;
                string message = string.Empty;
                player.Xp += XpGained;
            await SessionState.PushEvent($"You gain {XpGained} XP for defeating {dead.Name}.");

            while (player.Xp >= player.XpNeeded)
                {
                    player.Xp -= player.XpNeeded;
                    var levelResult = await GainLevel(player);
                    await SessionState.PushEvent(levelResult.Message);
                }
                int remain = player.XpNeeded - player.Xp;
                await _characterService.SavePlayer(player);
            await SessionState.PushEvent($"{remain} XP remaining for next level!");
                await SessionState.NotifyStateChange();
                
            }


            public void ProcessCombatRounds()
            {
                // Future expansion
            }


    }
}
