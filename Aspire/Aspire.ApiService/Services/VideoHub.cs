namespace Aspire.ApiService.Services;

public class VideoHub( ModelDbContext context, ILogger<VideoHub> logger) : Hub<IVideoClient> {
    private static readonly Dictionary<string, string?> PlayerQueue = [];
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static readonly Dictionary<string, string> PlayerNames = new();

    public async Task JoinQueue(string playerName, string? previousPlayer) {
        await Semaphore.WaitAsync();
        try {
            PlayerQueue.TryAdd(Context.ConnectionId, previousPlayer);
            PlayerNames[Context.ConnectionId] = playerName;

            await TryMatchPlayers();
        }
        finally {
            Semaphore.Release();
        }

        logger.LogInformation("Currently {PlayerCount} players in the queue", PlayerQueue.Count);
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);

        await Semaphore.WaitAsync();
        try {
            PlayerQueue.Remove(Context.ConnectionId);
            PlayerNames.Remove(Context.ConnectionId);
        }
        finally {
            Semaphore.Release();
        }
    }

    private async Task TryMatchPlayers() {
        logger.LogInformation("Trying to match players");

        if (PlayerQueue.Count < 2) return;

        var matchedPlayers = new List<string>();

        foreach (var (player, lastMatched) in PlayerQueue) {
            // Skip if player is already matched
            if (matchedPlayers.Contains(player)) continue;

            var otherPlayer = PlayerQueue.FirstOrDefault(p =>
                p.Key != player && p.Key != lastMatched && !matchedPlayers.Contains(p.Key));
            if (otherPlayer.Key is null) continue;

            matchedPlayers.Add(player);
            matchedPlayers.Add(otherPlayer.Key);

            logger.LogInformation("Match found between {Player1} and {Player2}", player, otherPlayer.Key);

            var gameId = Guid.NewGuid().ToString();
            await Clients.Client(player).MatchFound(gameId, otherPlayer.Key);
            await Clients.Client(otherPlayer.Key).MatchFound(gameId, player);

            await context.OngoingChads.AddAsync(new OngoingChad { RoomId = gameId });
            await context.SaveChangesAsync();
        }

        // Remove matched players from the queue
        foreach (var player in matchedPlayers) PlayerQueue.Remove(player);
    }


    public async Task JoinRoom(string roomId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendData(byte[] data, string roomId) {
        await Clients.OthersInGroup(roomId).ReceiveData(data);
    }

    public async Task LeaveRoom(string roomId) {
        await Clients.Group(roomId).Stop();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        await context.OngoingChads.Where(c => c.RoomId == roomId).ExecuteDeleteAsync();
    }

    public async Task Next(string oldRoomId, string prevPlayerId) {
        await Semaphore.WaitAsync();
        string p1Name;
        string p2Name;
        try {
            p1Name = PlayerNames[Context.ConnectionId];
            p2Name = PlayerNames[prevPlayerId];
        }
        finally {
            Semaphore.Release();
        }
        
        await LeaveRoom(oldRoomId);
        await JoinQueue(p1Name, prevPlayerId);
        await UpdateStats(p1Name, p2Name);
    }

    private async Task UpdateStats(string p1Name, string p2Name) {
        var p1 = await context.LeaderBoards
            .FirstOrDefaultAsync(p => p.PlayerName == p1Name);
        var p2 = await context.LeaderBoards
            .FirstOrDefaultAsync(p => p.PlayerName == p2Name);

        if (p1 is null) {
            p1 = new LeaderBoard {PlayerName = p1Name};
            context.LeaderBoards.Add(p1);
        }

        if (p2 is null) {
            p2 = new LeaderBoard {PlayerName = p2Name};
            context.LeaderBoards.Add(p2);
        }

        p1.SkippedOthers++;
        p2.SkippedByOthers++;

        await context.SaveChangesAsync();
    }
}

public interface IVideoClient {
    Task ReceiveData(byte[] data);
    Task MatchFound(string gameId, string otherPlayerId);
    
    Task Stop();
}