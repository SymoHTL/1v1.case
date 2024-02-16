namespace Aspire.ApiService.Services;

public class VideoHub(ILogger<VideoHub> logger) : Hub<IVideoClient> {
    private static readonly Dictionary<string, string?> PlayerQueue = [];
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    // ReSharper disable once ReplaceAsyncWithTaskReturn
    public async Task JoinQueue(string? previousPlayer) {
        logger.LogInformation("Player {ConnectionId} joined the queue", Context.ConnectionId);

        await Semaphore.WaitAsync();
        try {
            PlayerQueue.TryAdd(Context.ConnectionId, previousPlayer);
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
            PlayerQueue.Remove(Context.ConnectionId);
        }
        finally {
            Semaphore.Release();
        }
    }

    private async Task TryMatchPlayers() {
        logger.LogInformation("Trying to match players");

        await Semaphore.WaitAsync();
        try {
            if (PlayerQueue.Count < 2) return;

            var matchedPlayers = new List<string>();

            foreach (var (player, lastMatched) in PlayerQueue) {
                // Skip if player is already matched
                if (matchedPlayers.Contains(player)) continue;

                var otherPlayer = PlayerQueue.FirstOrDefault(p => p.Key != player && p.Key != lastMatched && !matchedPlayers.Contains(p.Key));
                if (otherPlayer.Key is null) continue;

                matchedPlayers.Add(player);
                matchedPlayers.Add(otherPlayer.Key);

                logger.LogInformation("Match found between {Player1} and {Player2}", player, otherPlayer.Key);

                var gameId = Guid.NewGuid().ToString();
                await Clients.Client(player).MatchFound(gameId, otherPlayer.Key);
                await Clients.Client(otherPlayer.Key).MatchFound(gameId, player);
            }

            // Remove matched players from the queue
            foreach (var player in matchedPlayers) {
                PlayerQueue.Remove(player);
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error while trying to match players");
        }
        finally {
            Semaphore.Release();
        }
    }


    public async Task JoinRoom(string roomId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendData(byte[] data, string roomId) {
        await Clients.OthersInGroup(roomId).ReceiveData(data);
    }

    public async Task LeaveRoom(string roomId) {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
}

public interface IVideoClient {
    Task ReceiveData(byte[] data);
    Task MatchFound(string gameId, string otherPlayerId);
}