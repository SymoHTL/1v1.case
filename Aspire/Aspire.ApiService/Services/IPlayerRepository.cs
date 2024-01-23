namespace Aspire.ApiService.Services;

public interface IPlayerRepository {
    
    Task<Player?> GetPlayer(string connId);
    Task AddPlayer(Player player);
    Task RemovePlayer(string connId);
}