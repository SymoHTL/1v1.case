namespace Aspire.ApiService.Services;

public class GameHub(ILogger<CsgoGameHub> logger, IPlayerRepository playerRepository) : Hub<IGameClient> {
    public async Task JoinGame(string gameId, string playerName) {
        logger.LogInformation("Player {ConnectionId} joined game {GameId}", Context.ConnectionId, gameId);
        var p = await playerRepository.AddPlayer(new Player(Context.ConnectionId, gameId, playerName));
        await Clients.Caller.SetLocalPlayer(p);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).PlayerJoined(p);
    }
}