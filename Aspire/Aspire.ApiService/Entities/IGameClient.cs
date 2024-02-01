namespace Aspire.ApiService.Entities;

public interface ICsgoClient {
    Task PlayerJoined(Player player);
    Task PlayerLeft(string userId);
    
    Task SetLocalPlayer(Player player);
    Task ReceiveBetChange(string userId, float bet);
    Task ReceiveCaseChange(string userId, string caseId);
    Task ReceiveReadyChange(string userId, bool ready);
    Task ReceiveReadyReset(List<string> userId, bool ready);
    Task StartRound(Dictionary<string, List<string>> results);
}