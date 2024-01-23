using System.ComponentModel.DataAnnotations;

namespace Aspire.ApiService.Entities;

public class Player {
    public Player(string contextConnectionId, string gameId) {
        ConnId = contextConnectionId;
        GameId = gameId;
    }

    public Player() { }

    [Key] [StringLength(128)] public string ConnId { get; set; } = null!;

    [StringLength(36)] public string GameId { get; set; } = null!;
}