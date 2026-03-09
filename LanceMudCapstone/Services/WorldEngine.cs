namespace LanceMudCapstone.Services
{
    public class WorldEngine : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;
        private readonly int _tickRateMs = 10000; // updated to 10
        private bool _isTicking = false;

        public WorldEngine(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Tick, null, 0, _tickRateMs);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            return Task.CompletedTask;
        }
        private async void Tick(object? state)
        {
            await TickAsync();
        }
        private async Task TickAsync()
        {
            if (_isTicking) return;
            _isTicking = true;
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var combat = scope.ServiceProvider.GetRequiredService<ICombatService>();
                var mobs = scope.ServiceProvider.GetRequiredService<MobService>();
                var effects = scope.ServiceProvider.GetRequiredService<EffectService>();
                var respawn = scope.ServiceProvider.GetRequiredService<RespawnService>();

                Console.WriteLine($"[WorldEngine] Tick at {DateTime.Now}");
                effects.ProcessEffects();
                combat.ProcessCombatRounds();
                mobs.ProcessMobAI();
                await respawn.AutoLogoutStaleCharacter();
                await respawn.ProcessRespawns();
                await respawn.TickHeal();
            }
            finally
            {
                _isTicking = false;
            }

        }
    }
}
