using Aspire.ApiService.Configuration;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Aspire.ApiService.Services;

public class PlayerRepository(ModelDbContext context) : IPlayerRepository {
    private readonly DbSet<Player> _players = context.Players;

    public async Task<Player?> GetPlayer(string connId) {
        return await _players.FindAsync(connId);
    }

    public async Task<Player> AddPlayer(Player player) {
        var p = _players.Add(player);
        await context.SaveChangesAsync();
        return p.Entity;
    }

    public async Task RemovePlayer(string connId) {
        var player = await _players.FindAsync(connId);
        if (player != null) _players.Remove(player);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePlayer(Player player) {
        _players.Update(player);
        await context.SaveChangesAsync();
    }

    public async Task<List<Player>> GetPlayers(string groupId) {
        return await _players.Where(p => p.GameId == groupId).ToListAsync();
    }

    public async Task UpdatePlayers(List<Player> players) {
        _players.UpdateRange(players);
        await context.SaveChangesAsync();
    }
}