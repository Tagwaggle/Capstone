using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services
{
    public class WorldEngine
    {
        private readonly ICombatService _combat;
        private readonly MobService _mobs;
        private readonly EffectService _effects;
        private readonly RespawnService _respawn;

        private Timer? _timer;
        private readonly int _tickRateMs = 10000; // updated to 10

        public WorldEngine(
            ICombatService combat,
            MobService mobs,
            EffectService effects,
            RespawnService respawn)
        {
            _combat = combat;
            _mobs = mobs;
            _effects = effects;
            _respawn = respawn;
        }

        public void Start()
        {
            _timer = new Timer(Tick, null, 0, _tickRateMs);
        }

        private void Tick(object? state)
        {
            Console.WriteLine($"[WorldEngine] Tick at {DateTime.Now}");
            _effects.ProcessEffects();
            _combat.ProcessCombatRounds();
            _mobs.ProcessMobAI();
            _respawn.ProcessRespawns();
        }
    }
}
