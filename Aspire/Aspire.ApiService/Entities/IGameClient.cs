namespace Aspire.ApiService.Entities;

public interface IGameClient {
    Task PlayerJoined(string userId);
    Task ReceiveGameUpdate(string userId, object gameData);
}