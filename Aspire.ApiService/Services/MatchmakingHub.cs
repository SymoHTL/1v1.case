namespace Aspire.ApiService.Services;

public class MatchmakingHub(ILogger<MatchmakingHub> logger)
    : Hub<IMatchmakingClient> {
    private static List<string> _playerQueue = new();
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    // ReSharper disable once ReplaceAsyncWithTaskReturn
    public async Task JoinQueue() {
        logger.LogInformation("Player {ConnectionId} joined the queue", Context.ConnectionId);

        await Semaphore.WaitAsync();
        try {
            if (_playerQueue.Contains(Context.ConnectionId)) return;
            _playerQueue.Add(Context.ConnectionId);
        }
        finally {
            Semaphore.Release();
        }

        await TryMatchPlayers();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);

        await Semaphore.WaitAsync();
        try {
            _playerQueue.Remove(Context.ConnectionId);
        }
        finally {
            Semaphore.Release();
        }
    }

    private async Task TryMatchPlayers() {
        logger.LogInformation("Trying to match players");

        await Semaphore.WaitAsync();
        try {
            if (_playerQueue.Count < 2) return;
            var p1 = _playerQueue[0];
            var p2 = _playerQueue[1];
            _playerQueue.RemoveRange(0, 2);
            logger.LogInformation("Match found between {Player1} and {Player2}", p1, p2);

            var gameId = Guid.NewGuid().ToString();

            await Clients.Client(p1).MatchFound(gameId);
            await Clients.Client(p2).MatchFound(gameId);
        }
        finally {
            Semaphore.Release();
        }
    }
}