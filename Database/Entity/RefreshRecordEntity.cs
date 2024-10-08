using System.ComponentModel.DataAnnotations;

namespace Domain.OAuth;

public class RefreshRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long LoginRecordId { get; init; }
    
    public required LoginRecordEntity LoginRecord { get; init; }
    
    [StringLength(1028, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string JwtId { get; init; }
    
    public required List<AccessRecordEntity> AccessRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}