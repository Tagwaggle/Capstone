using LanceMudCapstone.DTOs;

namespace LanceMudCapstone.Models
{
    public class AbilityBook
    {
        public static List<AbilityDto> GetByClass(string className)
        {
            return className.ToLower() switch
            {
                "paladin" => Paladin,
                "mage" => Mage,
                "warrior" => Warrior,
                "cleric" => Cleric,
                "rogue" => Rogue,
                "hunter" => Hunter,
                "knight" => Knight,
                _ => new List<AbilityDto>()
            };
        }
        public static AbilityDto GetBaseAttack(string className) => className.ToLower() switch
        {
            "warrior" or "knight" => new AbilityDto
            { Name = "Strike", LevelRequirement = 1, DamageFormula = "1d8", CooldownSeconds = 0, StaminaCost = 0, Description = "a striking blow", Type = "Physical" },
            "mage" => new AbilityDto
            { Name = "Magic Bolt", LevelRequirement = 1, DamageFormula = "1d6", CooldownSeconds = 0, ManaCost = 0, Description = "an arcane bolt", Type = "Arcane" },
            "cleric" or "paladin" => new AbilityDto
            { Name = "Smite", LevelRequirement = 1, DamageFormula = "1d8", CooldownSeconds = 0, ManaCost = 0, Description = "smiting from above", Type = "Holy" },
            "rogue" or "hunter" => new AbilityDto
            { Name = "Quick Attack", LevelRequirement = 1, DamageFormula = "1d6", CooldownSeconds = 0, StaminaCost = 0, Description = "a quick strike from the shadows", Type = "Physical" },
            _ => new AbilityDto { Name = "Strike", LevelRequirement = 1, DamageFormula = "1d8", CooldownSeconds = 0, StaminaCost = 0, Description = "easily", Type = "Physical" }
        };
        public static readonly List<AbilityDto> Warrior = new()
        {
            new AbilityDto { Name="Kick", LevelRequirement=1, DamageFormula="2d10", CooldownSeconds=3, StaminaCost=5, Description="Enzuigiri kick to defender.name", Type="Physical" },
            new AbilityDto { Name="Punch", LevelRequirement=1, DamageFormula="1d10", CooldownSeconds=2, StaminaCost=3, Description="Hook to defender.name's body", Type="Physical" },
            new AbilityDto { Name="Defend", LevelRequirement=1, DamageFormula="0", CooldownSeconds=3, StaminaCost=4, Description="You brace and block all incoming damage", Type="Defensive" },
            new AbilityDto { Name="Shield Bash", LevelRequirement=2, DamageFormula="2d8", CooldownSeconds=3, StaminaCost=6, Description="You slam your shield into defender.name", Type="Physical" },
            new AbilityDto { Name="Heavy Swing", LevelRequirement=2, DamageFormula="2d12", CooldownSeconds=4, StaminaCost=7, Description="Overhand strike crashes down on defender.name", Type="Physical" },
            new AbilityDto { Name="Taunt", LevelRequirement=2, DamageFormula="0", CooldownSeconds=5, StaminaCost=4, Description="You taunt defender.name into attacking you", Type="Control" },
            new AbilityDto { Name="Brace", LevelRequirement=3, DamageFormula="0", CooldownSeconds=5, StaminaCost=5, Description="You dig in and brace for incoming attacks", Type="Defensive" },
            new AbilityDto { Name="Second Wind", LevelRequirement=3, DamageFormula="Heal 2d10", CooldownSeconds=6, StaminaCost=0, Description="You catch your breath and push through the pain", Type="Healing" },
            new AbilityDto { Name="Hamstring", LevelRequirement=4, DamageFormula="1d8", CooldownSeconds=4, StaminaCost=5, Description="You slash low, crippling defender.name’s movement", Type="Control" },
            new AbilityDto { Name="Disarm", LevelRequirement=4, DamageFormula="1d6", CooldownSeconds=5, StaminaCost=6, Description="You knock the weapon from defender.name’s grasp", Type="Control" },
            new AbilityDto { Name="Cleave", LevelRequirement=5, DamageFormula="2d10", CooldownSeconds=4, StaminaCost=8, Description="You spin in a wide arc, striking everyone nearby", Type="Physical" },
            new AbilityDto { Name="Execute", LevelRequirement=5, DamageFormula="3d12", CooldownSeconds=8, StaminaCost=10, Description="You deliver a brutal finishing blow to defender.name", Type="Physical" },
            new AbilityDto { Name="Battle Rage", LevelRequirement=6, DamageFormula="0", CooldownSeconds=6, StaminaCost=8, Description="You enter a furious battle trance", Type="Buff" }
        };

        public static readonly List<AbilityDto> Mage = new()
        {
            new AbilityDto { Name="Magic Missile", LevelRequirement=1, DamageFormula="2d6", CooldownSeconds=2, ManaCost=5, Description="A bolt of arcane energy strikes defender.name", Type="Arcane" },
            new AbilityDto { Name="Spark", LevelRequirement=1, DamageFormula="1d8", CooldownSeconds=1, ManaCost=3, Description="Crackling sparks leap from your fingers", Type="Arcane" },
            new AbilityDto { Name="Arcane Shield", LevelRequirement=1, DamageFormula="0", CooldownSeconds=4, ManaCost=6, Description="A shimmering barrier surrounds you", Type="Defensive" },
            new AbilityDto { Name="Frostbite", LevelRequirement=2, DamageFormula="1d8", CooldownSeconds=3, ManaCost=5, Description="Ice creeps over defender.name’s limbs", Type="Cold" },
            new AbilityDto { Name="Fire Bolt", LevelRequirement=2, DamageFormula="2d8", CooldownSeconds=3, ManaCost=6, Description="A streak of flame scorches defender.name", Type="Fire" },
            new AbilityDto { Name="Lightning Jolt", LevelRequirement=3, DamageFormula="2d10", CooldownSeconds=4, ManaCost=7, Description="Lightning snaps violently into defender.name", Type="Lightning" },
            new AbilityDto { Name="Mana Surge", LevelRequirement=3, DamageFormula="Restore 2d10 mana", CooldownSeconds=6, ManaCost=0, Description="You draw deeply from ambient magic", Type="Utility" },
            new AbilityDto { Name="Ice Lance", LevelRequirement=4, DamageFormula="3d8", CooldownSeconds=4, ManaCost=8, Description="A razor-sharp shard of ice impales defender.name", Type="Cold" },
            new AbilityDto { Name="Chain Lightning", LevelRequirement=4, DamageFormula="2d8 (chain)", CooldownSeconds=5, ManaCost=10, Description="Lightning arcs wildly between enemies", Type="Lightning" },
            new AbilityDto { Name="Fireball", LevelRequirement=5, DamageFormula="3d10 (area)", CooldownSeconds=6, ManaCost=12, Description="A roaring explosion engulfs the room in flames", Type="Fire" },
            new AbilityDto { Name="Arcane Blast", LevelRequirement=6, DamageFormula="4d10", CooldownSeconds=7, ManaCost=14, Description="Raw magic detonates against defender.name", Type="Arcane" },
            new AbilityDto { Name="Meteor Shard", LevelRequirement=7, DamageFormula="5d10", CooldownSeconds=8, ManaCost=16, Description="A fragment of a falling star crashes into defender.name", Type="Fire" },
            new AbilityDto { Name="Dragon’s Breath", LevelRequirement=13, DamageFormula="6d12 (all)", CooldownSeconds=10, ManaCost=20, Description="You exhale devastating draconic fire across the battlefield", Type="Fire" }
        };

        public static readonly List<AbilityDto> Cleric = new()
        {
            new AbilityDto { Name="Smite", LevelRequirement=1, DamageFormula="2d8", CooldownSeconds=3, ManaCost=4, Description="Holy energy smites defender.name", Type="Holy" },
            new AbilityDto { Name="Minor Heal", LevelRequirement=2, DamageFormula="Heal 2d8", CooldownSeconds=3, ManaCost=5, Description="A soft holy light mends your wounds", Type="Healing" },
            new AbilityDto { Name="Radiant Strike", LevelRequirement=3, DamageFormula="2d10", CooldownSeconds=3, ManaCost=6, Description="Your weapon glows as it strikes defender.name", Type="Holy" },
            new AbilityDto { Name="Heal", LevelRequirement=4, DamageFormula="Heal 3d10", CooldownSeconds=4, ManaCost=8, Description="Divine power restores flesh and spirit", Type="Healing" },
            new AbilityDto { Name="Holy Fire", LevelRequirement=5, DamageFormula="3d10", CooldownSeconds=4, ManaCost=10, Description="Sacred flames burn defender.name", Type="Holy" },
            new AbilityDto { Name="Prayer of Mending", LevelRequirement=6, DamageFormula="Heal 2d10 (all)", CooldownSeconds=6, ManaCost=12, Description="A prayer echoes through the party", Type="Healing" },
            new AbilityDto { Name="Judgement", LevelRequirement=7, DamageFormula="3d12", CooldownSeconds=5, ManaCost=12, Description="You call divine judgement upon defender.name", Type="Holy" },
            new AbilityDto { Name="Greater Heal", LevelRequirement=8, DamageFormula="Heal 4d12", CooldownSeconds=6, ManaCost=14, Description="Brilliant light floods the target with life", Type="Healing" },
            new AbilityDto { Name="Consecrate", LevelRequirement=9, DamageFormula="2d10 (2 rounds)", CooldownSeconds=6, ManaCost=12, Description="Holy ground sears all enemies nearby", Type="Holy" },
            new AbilityDto { Name="Bless", LevelRequirement=10, DamageFormula="0", CooldownSeconds=8, ManaCost=10, Description="+2 attack, +2 defense for 3 rounds", Type="Buff" },
            new AbilityDto { Name="Wrath of the Faithful", LevelRequirement=11, DamageFormula="4d12", CooldownSeconds=7, ManaCost=14, Description="Righteous fury crashes down on defender.name", Type="Holy" },
            new AbilityDto { Name="Divine Intervention", LevelRequirement=12, DamageFormula="Heal Full", CooldownSeconds=10, ManaCost=20, Description="A miracle answers your desperate plea", Type="Healing" },
            new AbilityDto { Name="Apocalypse Light", LevelRequirement=13, DamageFormula="5d12 (all)", CooldownSeconds=12, ManaCost=22, Description="Blinding holy light purges the battlefield", Type="Holy" }
        };

        public static readonly List<AbilityDto> Rogue = new()
        {
            new AbilityDto { Name="Stab", LevelRequirement=1, DamageFormula="2d8", CooldownSeconds=2, StaminaCost=4, Description="You drive a quick blade into defender.name", Type="Physical" },
            new AbilityDto { Name="Throw Dagger", LevelRequirement=1, DamageFormula="1d10", CooldownSeconds=2, StaminaCost=3, Description="A dagger whistles through the air", Type="Physical" },
            new AbilityDto { Name="Backstab", LevelRequirement=2, DamageFormula="3d10", CooldownSeconds=4, StaminaCost=6, Description="You strike a vital spot on defender.name", Type="Physical" },
            new AbilityDto { Name="Dodge", LevelRequirement=2, DamageFormula="0", CooldownSeconds=4, StaminaCost=4, Description="You nimbly evade incoming attacks", Type="Defensive" },
            new AbilityDto { Name="Poison Blade", LevelRequirement=3, DamageFormula="2d8 + 1d6(3 rounds)", CooldownSeconds=4, StaminaCost=6, Description="Your blade leaves a sickly poison", Type="Poison" },
            new AbilityDto { Name="Dirty Trick", LevelRequirement=3, DamageFormula="1d6", CooldownSeconds=3, StaminaCost=4, Description="You fight dirty, blinding defender.name", Type="Control" },
            new AbilityDto { Name="Sap", LevelRequirement=4, DamageFormula="1d8", CooldownSeconds=5, StaminaCost=5, Description="You knock defender.name senseless", Type="Control" },
            new AbilityDto { Name="Smoke Bomb", LevelRequirement=4, DamageFormula="0", CooldownSeconds=6, StaminaCost=6, Description="Smoke erupts, hiding your movements", Type="Utility" },
            new AbilityDto { Name="Eviscerate", LevelRequirement=5, DamageFormula="4d10", CooldownSeconds=6, StaminaCost=8, Description="A brutal flurry tears into defender.name", Type="Physical" },
            new AbilityDto { Name="Crippling Strike", LevelRequirement=5, DamageFormula="2d10", CooldownSeconds=5, StaminaCost=6, Description="You maim defender.name’s fighting ability", Type="Control" },
            new AbilityDto { Name="Shadowstep", LevelRequirement=6, DamageFormula="0", CooldownSeconds=6, StaminaCost=6, Description="You vanish and reappear behind your foe", Type="Utility" },
            new AbilityDto { Name="Fan of Knives", LevelRequirement=6, DamageFormula="2d8 (all)", CooldownSeconds=7, StaminaCost=8, Description="Blades fly outward in every direction", Type="Physical" },
            new AbilityDto { Name="Assassinate", LevelRequirement=13, DamageFormula="6d12", CooldownSeconds=10, StaminaCost=12, Description="You deliver a flawless killing strike", Type="Physical" }
        };

        public static readonly List<AbilityDto> Hunter = new()
        {
            new AbilityDto { Name="Aimed Shot", LevelRequirement=1, DamageFormula="2d8", CooldownSeconds=2, StaminaCost=4, Description="You carefully line up a shot", Type="Ranged" },
            new AbilityDto { Name="Quick Shot", LevelRequirement=1, DamageFormula="1d10", CooldownSeconds=1, StaminaCost=3, Description="You fire a fast arrow", Type="Ranged" },
            new AbilityDto { Name="Mark Target", LevelRequirement=2, DamageFormula="0", CooldownSeconds=4, StaminaCost=4, Description="+2 damage vs defender.name for 3 rounds", Type="Debuff" },
            new AbilityDto { Name="Evasive Roll", LevelRequirement=2, DamageFormula="0", CooldownSeconds=4, StaminaCost=4, Description="You roll clear of incoming attacks", Type="Defensive" },
            new AbilityDto { Name="Poison Arrow", LevelRequirement=3, DamageFormula="2d8 + 1d6(3 rounds)", CooldownSeconds=4, StaminaCost=5, Description="A poisoned arrow sinks into defender.name", Type="Poison" },
            new AbilityDto { Name="Snare Trap", LevelRequirement=3, DamageFormula="0", CooldownSeconds=5, StaminaCost=5, Description="defender.name is caught in a hidden trap", Type="Control" },
            new AbilityDto { Name="Power Shot", LevelRequirement=4, DamageFormula="3d10", CooldownSeconds=5, StaminaCost=7, Description="You loose a powerful arrow", Type="Ranged" },
            new AbilityDto { Name="Camouflage", LevelRequirement=4, DamageFormula="0", CooldownSeconds=6, StaminaCost=5, Description="You blend into your surroundings", Type="Utility" },
            new AbilityDto { Name="Volley", LevelRequirement=5, DamageFormula="2d10 (all)", CooldownSeconds=6, StaminaCost=8, Description="Arrows rain down across the battlefield", Type="Ranged" },
            new AbilityDto { Name="Crippling Shot", LevelRequirement=5, DamageFormula="2d10", CooldownSeconds=5, StaminaCost=6, Description="You hobble defender.name", Type="Control" },
            new AbilityDto { Name="Tracking Instincts", LevelRequirement=6, DamageFormula="0", CooldownSeconds=7, StaminaCost=6, Description="+2 accuracy, cannot miss for 3 rounds", Type="Buff" },
            new AbilityDto { Name="Explosive Trap", LevelRequirement=6, DamageFormula="3d10 (area)", CooldownSeconds=7, StaminaCost=8, Description="A hidden trap detonates violently", Type="Fire" },
            new AbilityDto { Name="Kill Shot", LevelRequirement=13, DamageFormula="5d12", CooldownSeconds=9, StaminaCost=10, Description="You execute your wounded prey", Type="Ranged" }
        };

        public static readonly List<AbilityDto> Paladin = new()
{
    new AbilityDto { Name="Smite Strike", LevelRequirement=1, DamageFormula="2d10", CooldownSeconds=3, ManaCost=4, Description="You smite defender.name with righteous force", Type="Holy" },
    new AbilityDto { Name="Lay on Hands", LevelRequirement=1, DamageFormula="Heal 2d10", CooldownSeconds=4, ManaCost=6, Description="Holy light flows from your hands", Type="Healing" },
    new AbilityDto { Name="Shield of Faith", LevelRequirement=2, DamageFormula="0", CooldownSeconds=5, ManaCost=6, Description="A divine shield surrounds you", Type="Defensive" },
    new AbilityDto { Name="Holy Rebuke", LevelRequirement=2, DamageFormula="2d8", CooldownSeconds=4, ManaCost=5, Description="Holy energy lashes back at defender.name", Type="Holy" },
    new AbilityDto { Name="Zealous Strike", LevelRequirement=3, DamageFormula="3d10", CooldownSeconds=4, ManaCost=7, Description="You strike defender.name with unwavering conviction", Type="Holy" },
    new AbilityDto { Name="Aura of Courage", LevelRequirement=3, DamageFormula="0", CooldownSeconds=6, ManaCost=8, Description="Party immune to fear for 3 rounds", Type="Buff" },
    new AbilityDto { Name="Judging Blow", LevelRequirement=4, DamageFormula="2d12", CooldownSeconds=5, ManaCost=8, Description="You expose defender.name’s weakness", Type="Holy" },
    new AbilityDto { Name="Divine Ward", LevelRequirement=4, DamageFormula="0", CooldownSeconds=6, ManaCost=10, Description="Negate next incoming attack", Type="Defensive" },
    new AbilityDto { Name="Consecrated Cleave", LevelRequirement=5, DamageFormula="2d10 (all)", CooldownSeconds=6, ManaCost=10, Description="Holy steel sweeps through nearby foes", Type="Holy" },
    new AbilityDto { Name="Oathbound Resolve", LevelRequirement=5, DamageFormula="0", CooldownSeconds=8, ManaCost=12, Description="You cannot drop below 1 HP for 2 rounds", Type="Buff" },
    new AbilityDto { Name="Wrath of Light", LevelRequirement=6, DamageFormula="4d12", CooldownSeconds=7, ManaCost=14, Description="Blazing holy power crashes into defender.name", Type="Holy" },
    new AbilityDto { Name="Redemption", LevelRequirement=6, DamageFormula="Heal 3d12", CooldownSeconds=8, ManaCost=16, Description="Divine mercy restores body and soul", Type="Healing" },
    new AbilityDto { Name="Divine Judgement", LevelRequirement=13, DamageFormula="5d12", CooldownSeconds=10, ManaCost=20, Description="Final judgement descends upon defender.name", Type="Holy" }
};
        public static readonly List<AbilityDto> Knight = new()
{
    new AbilityDto { Name="Shield Strike", LevelRequirement=1, DamageFormula="2d8", CooldownSeconds=3, StaminaCost=4, Description="You strike defender.name with shield and steel", Type="Physical" },
    new AbilityDto { Name="Guard", LevelRequirement=1, DamageFormula="0", CooldownSeconds=4, StaminaCost=4, Description="You redirect the next attack to yourself", Type="Defensive" },
    new AbilityDto { Name="Tactical Slash", LevelRequirement=2, DamageFormula="2d10", CooldownSeconds=3, StaminaCost=5, Description="A precise, disciplined strike lands on defender.name", Type="Physical" },
    new AbilityDto { Name="Defensive Stance", LevelRequirement=2, DamageFormula="0", CooldownSeconds=5, StaminaCost=5, Description="50% damage reduction for 2 rounds", Type="Defensive" },
    new AbilityDto { Name="Commanding Shout", LevelRequirement=3, DamageFormula="0", CooldownSeconds=6, StaminaCost=6, Description="+2 attack for allies for 2 rounds", Type="Buff" },
    new AbilityDto { Name="Shield Wall", LevelRequirement=3, DamageFormula="0", CooldownSeconds=7, StaminaCost=7, Description="Reduce all party damage by 25% for 2 rounds", Type="Buff" },
    new AbilityDto { Name="Break Formation", LevelRequirement=4, DamageFormula="2d12", CooldownSeconds=5, StaminaCost=6, Description="You exploit a weakness in defender.name’s guard", Type="Physical" },
    new AbilityDto { Name="Hold the Line", LevelRequirement=4, DamageFormula="0", CooldownSeconds=6, StaminaCost=6, Description="Cannot be stunned or knocked down for 2 rounds", Type="Buff" },
    new AbilityDto { Name="Sweeping Advance", LevelRequirement=5, DamageFormula="2d10 (all)", CooldownSeconds=6, StaminaCost=8, Description="You press forward, driving enemies back", Type="Physical" },
    new AbilityDto { Name="Rally", LevelRequirement=5, DamageFormula="Heal 2d10", CooldownSeconds=7, StaminaCost=8, Description="Your presence restores courage and strength", Type="Healing" },
    new AbilityDto { Name="Knight’s Challenge", LevelRequirement=6, DamageFormula="0", CooldownSeconds=6, StaminaCost=6, Description="Forces defender.name to attack you for 2 rounds", Type="Control" },
    new AbilityDto { Name="Unyielding Oath", LevelRequirement=6, DamageFormula="0", CooldownSeconds=8, StaminaCost=8, Description="Cannot drop below 1 HP for 2 rounds", Type="Buff" },
    new AbilityDto { Name="Oathbound Strike", LevelRequirement=13, DamageFormula="5d12", CooldownSeconds=10, StaminaCost=12, Description="You deliver a decisive, oath-driven blow to defender.name", Type="Physical" }
};


    }
}
