namespace Aspire.ApiService.Entities;

[Table("OngoingChads")]
public class OngoingChad {
    [Key]
    [StringLength(36)]
    public string RoomId { get; set; } = null!;
}
