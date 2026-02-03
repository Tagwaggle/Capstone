namespace LanceMudCapstone.DTOs

{    public class CombatResult
    {
        public PlayerCharacterDto Attacker { get; set; } = default!;
        public PlayerCharacterDto Defender { get; set; } = default!;
        public List<string> Log { get; set; } = new();
    }

    public class CombatRoundResult
    {
        public PlayerCharacterDto Player { get; set; } = default!;
        public PlayerCharacterDto Mob { get; set; } = default!;
        public List<string> Log { get; set; } = new();
    }
}
