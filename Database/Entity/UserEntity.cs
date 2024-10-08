using System.ComponentModel.DataAnnotations;

namespace Domain.OAuth;

public class UserEntity
{
    public long Id { get; set; }
    
    [StringLength(512, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string ProviderId { get; init; }
    
    public required OAuthProvider AuthenticationProvider { get; init; }
    
    [StringLength(256, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Username { get; set; }
    
    [StringLength(2056, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string AvatarUrl { get; set; }
    
    [StringLength(512, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Email { get; set; }
    
    public required List<LoginRecordEntity> LoginRecords { get; init; }
    
    public required List<OldInformationRecordEntity> OldInformationRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}