namespace Shared.Entities;

public class Leaderboard {
    public string PlayerName { get; set; } = null!;
    public int SkippedOthers { get; set; }
    public int SkippedByOthers { get; set; }
}