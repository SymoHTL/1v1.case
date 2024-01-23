namespace Aspire.ApiService.Entities;

public interface IMatchmakingClient {
    Task MatchFound(string gameId);
}