using Shared.Entities;

namespace Aspire.ApiService.Services;

public interface IPlayerRepository {
    
    Task<Player?> GetPlayer(string connId);
    Task<Player> AddPlayer(Player player);
    Task RemovePlayer(string connId);
    Task UpdatePlayer(Player player);
    Task<List<Player>> GetPlayers(string groupId);
    Task UpdatePlayers(List<Player> players);
}