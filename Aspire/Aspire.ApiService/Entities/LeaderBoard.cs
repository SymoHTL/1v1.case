namespace Aspire.ApiService.Entities;

[Table("LeaderBoards")]
public class LeaderBoard {
    [Key][StringLength(50)]
    public string PlayerName { get; set; } = null!;
    public int SkippedOthers { get; set; }
    public int SkippedByOthers { get; set; }
}