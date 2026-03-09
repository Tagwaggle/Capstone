using LanceMudCapstone.DTOs;
using System.Text.RegularExpressions;

namespace LanceMudCapstone.Services
{
    public class AbilityResult
    {
        public string Message { get; set; } = string.Empty;
        public int Damage { get; set; }
        public int Healing { get; set; }
        public bool Success { get; set; } = true;

    }
    public class AbilityEngine
    {
        private readonly Random _rng = new();

        public AbilityResult ExecuteAbility(PlayerCharacterDto caster, PlayerCharacterDto target, AbilityDto ability)
        {
            if (ability.ManaCost > 0)
            {
                caster.Mana = Math.Max(0, caster.Mana - ability.ManaCost.Value);
            }
            if (ability.StaminaCost > 0)
            {
                caster.Stamina = Math.Max(0, caster.Stamina - ability.StaminaCost.Value);

            }
            var result = ParseFormula(ability.DamageFormula, caster, target);

            string msg = ability.Description.Replace("defender.name", target.Name)
                                            .Replace("attacker.name", caster.Name)
                                            .Replace("{damage}", result.Damage.ToString())
                                            .Replace("{healing}", result.Healing.ToString());


            if (result.Healing > 0)
            {
                target.Health = Math.Min(target.MaxHealth, target.Health + result.Healing);
            }
            if (result.Damage > 0)
            {
                target.Health -= result.Damage;
                if (target.Health <= 0)
                {
                    target.IsAlive = false;
                    target.Health = 0;
                    msg += $" {target.Name} has been defeated!";
                }
            }
            return new AbilityResult
            {
                Message = msg,
                Damage = result.Damage,
                Healing = result.Healing,
                Success = true
            };
        }
        private AbilityResult ParseFormula(string formula, PlayerCharacterDto caster, PlayerCharacterDto target)
        {
            var result = new AbilityResult();

            if (formula.StartsWith("Heal", StringComparison.OrdinalIgnoreCase))
            {
                string dice = formula.Replace("Heal", "").Trim();
                result.Healing = RollDiceExpression(dice, caster);
                return result;
            }

            if (formula.StartsWith("Restore", StringComparison.OrdinalIgnoreCase))
            {
                string dice = formula.Replace("Restore", "").Trim();
                result.Healing = RollDiceExpression(dice, caster);
                return result;
            }

            if (formula == "0")
                return result;

            result.Damage = RollDiceExpression(formula, caster);
            return result;
        }

        private int RollDiceExpression(string expr, PlayerCharacterDto caster)
        {
            expr = expr.Replace(" ", "");

            var statMatch = Regex.Match(expr, @"\+([A-Z]+)");
            int statBonus = 0;

            if (statMatch.Success)
            {
                string stat = statMatch.Groups[1].Value;
                statBonus = GetStatValue(caster, stat);
                expr = expr.Replace("+" + stat, "");
            }

            var diceMatch = Regex.Match(expr, @"(\d+)d(\d+)");
            if (!diceMatch.Success)
                return statBonus;

            int count = int.Parse(diceMatch.Groups[1].Value);
            int sides = int.Parse(diceMatch.Groups[2].Value);

            int total = 0;
            for (int i = 0; i < count; i++)
                total += _rng.Next(1, sides + 1);

            return total + statBonus;
        }

        private int GetStatValue(PlayerCharacterDto c, string stat)
        {
            return stat switch
            {
                "STR" => c.Strength,
                "DEX" => c.Dexterity,
                "CON" => c.Constitution,
                "INT" => c.Intelligence,
                "WIS" => c.Wisdom,
                "CHA" => c.Charisma,
                _ => 0
            };

        }
    }
}
