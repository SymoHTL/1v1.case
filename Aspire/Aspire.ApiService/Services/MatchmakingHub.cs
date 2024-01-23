using Microsoft.AspNetCore.SignalR;

namespace Aspire.ApiService.Services;

public class MatchmakingHub(ILogger<MatchmakingHub> logger)
    : Hub<IMatchmakingClient> {
    private static ConcurrentQueue<string> _playerQueue = new();
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    // ReSharper disable once ReplaceAsyncWithTaskReturn
    public async Task JoinQueue() {
        logger.LogInformation("Player {ConnectionId} joined the queue", Context.ConnectionId);

        _playerQueue.Enqueue(Context.ConnectionId);
        
        await TryMatchPlayers();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        
        await Semaphore.WaitAsync();
        _playerQueue = new ConcurrentQueue<string>(_playerQueue.Where(x => x != Context.ConnectionId));
        Semaphore.Release();
    }

    private async Task TryMatchPlayers() {
        logger.LogInformation("Trying to match players");
        
        await Semaphore.WaitAsync();
        if (_playerQueue.Count < 2) {
            Semaphore.Release();
            return;
        }
        
        var success = _playerQueue.TryDequeue(out var p1);
        if (!success) {
            Semaphore.Release();
            logger.LogInformation("Failed to dequeue players");
            return;
        }
        success = _playerQueue.TryDequeue(out var p2);
        if (!success) {
            Semaphore.Release();
            logger.LogInformation("Failed to dequeue players");
            return;
        }
        
        logger.LogInformation("Match found between {Player1} and {Player2}", p1, p2);
        
        var gameId = Guid.NewGuid().ToString();

        await Clients.Client(p1!).MatchFound(gameId);
        await Clients.Client(p2!).MatchFound(gameId);
    }

}