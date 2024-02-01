namespace Aspire.ApiService.Entities;

public interface IGameClient {
    Task PlayerJoined(Player player);
    
    Task PlayerLeft(string userId);
    Task SetLocalPlayer(Player player);
}