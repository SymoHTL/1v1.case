namespace Aspire.ApiService.Entities;

[Table("OngoingChads")]
public class OngoingChad {
    [Key]
    [StringLength(36)]
    public string RoomId { get; set; } = null!;

    [StringLength(36)]
    public string Player1 { get; set; } = null!;
    [StringLength(36)]
    public string Player2 { get; set; } = null!;
}
