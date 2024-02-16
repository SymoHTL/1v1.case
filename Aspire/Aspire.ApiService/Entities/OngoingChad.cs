namespace Aspire.ApiService.Entities;

[Table("OngoingChads")]
public class OngoingChad {
    [Key] public string RoomId { get; set; } = null!;
}