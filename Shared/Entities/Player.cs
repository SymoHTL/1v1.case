using System.ComponentModel.DataAnnotations;

namespace Shared.Entities;

public class Player {
    public Player(string contextConnectionId, string gameId, string playerName) {
        ConnId = contextConnectionId;
        GameId = gameId;
        Name = playerName;
    }

    public Player() {
    }

    [Key] [StringLength(128)] public string ConnId { get; set; } = null!;

    [StringLength(36)] public string GameId { get; set; } = null!;
    [Required]
    [StringLength(36)] public string Name { get; set; } = null!;
    public float Bet { get; set; }


    [StringLength(36)] public string? CaseId { get; set; }

    public bool Ready { get; set; }
}