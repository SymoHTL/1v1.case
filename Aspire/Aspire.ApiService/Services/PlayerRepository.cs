using Aspire.ApiService.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Aspire.ApiService.Services;

public class PlayerRepository(ModelDbContext context) : IPlayerRepository {
   private readonly DbSet<Player> _players = context.Players;

   public async Task<Player?> GetPlayer(string connId) {
      return await _players.FindAsync(connId);
   }

   public async Task AddPlayer(Player player) {
      await _players.AddAsync(player);
   }

   public async Task RemovePlayer(string connId) {
      var player = await _players.FindAsync(connId);
      if (player != null) _players.Remove(player);
   }
}