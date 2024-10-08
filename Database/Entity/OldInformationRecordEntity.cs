using System.ComponentModel.DataAnnotations;

namespace Domain.OAuth;

public class OldInformationRecordEntity
{
    public long Id { get; set; }
    
    public required long UserId { get; init; }
    
    public required UserEntity User { get; init; }
    
    public required OldInformation Type { get; init; }
    
    [StringLength(2056, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Information { get; init; }
    
    public required DateTime ReplacedUtc { get; init; }
}