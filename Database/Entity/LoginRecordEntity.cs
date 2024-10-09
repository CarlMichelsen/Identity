using System.ComponentModel.DataAnnotations;

namespace Database.Entity;

public class LoginRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long UserId { get; init; }
    
    public required UserEntity User { get; init; }
    
    [StringLength(1048576, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string OAuthJson { get; init; }
    
    public required List<RefreshRecordEntity> RefreshRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
    
    public DateTime? InvalidatedUtc { get; set; }
}