using System.ComponentModel.DataAnnotations;

namespace Domain.OAuth;

public class AccessRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long RefreshRecordId { get; init; }
    
    public required RefreshRecordEntity RefreshRecord { get; init; }
    
    [StringLength(1028, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string JwtId { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}