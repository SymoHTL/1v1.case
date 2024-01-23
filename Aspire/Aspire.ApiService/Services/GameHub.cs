using Microsoft.AspNetCore.SignalR;

namespace Aspire.ApiService.Services;

public class GameHub(ILogger<GameHub> logger, IPlayerRepository playerRepository) : Hub<IGameClient> {
    public async Task JoinGame(string gameId, string playerName) {
        logger.LogInformation("Player {ConnectionId} joined game {GameId}", Context.ConnectionId, gameId);
        await playerRepository.AddPlayer(new Player(Context.ConnectionId, gameId));
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).PlayerJoined(playerName);
    }

    public async Task SendGameUpdate(string gameId, string userId, object gameData) {
        logger.LogInformation("Received game update from {UserId} in game {GameId}", userId, gameId);
        // Send updates to the other player in the game
        await Clients.OthersInGroup(gameId).ReceiveGameUpdate(userId, gameData);
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);

        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);

        var player = await playerRepository.GetPlayer(Context.ConnectionId);
        if (player == null) return;
        await playerRepository.RemovePlayer(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.GameId);
    }
}