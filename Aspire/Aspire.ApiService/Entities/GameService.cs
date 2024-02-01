using Aspire.ApiService.Services;

namespace Aspire.ApiService.Entities;

public class GameService : IDisposable {
    private readonly IHubContext<GameHub> _gameHub;
    private bool _isGameRunning;
    private float _lastTick;
    private int _ticksPerSecond = 60;

    public Dictionary<string, Player3d> Players { get; set; } = [];

    public GameService(IHubContext<GameHub> gameHub) {
        _gameHub = gameHub;
    }

    public void StartGameLoop() {
        _isGameRunning = true;
        Task.Run(() => {
            while (_isGameRunning) {
                var now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                var delta = now - _lastTick;
                if (delta < 1000 / _ticksPerSecond) continue;
                _lastTick = now;
                UpdateGameState();
            }
            return Task.CompletedTask;
        });
    }

    private void UpdateGameState() {
    }
    

    public void Dispose() {
        _isGameRunning = false;
    }
}