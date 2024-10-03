using System.ComponentModel.DataAnnotations;
using Domain.OAuth;

namespace Database.Entity;

public class UserEntity
{
    public required long Id { get; set; }
    
    [StringLength(512, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string ProviderId { get; init; }
    
    public required OAuthProvider AuthenticationProvider { get; init; }
    
    [StringLength(256, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Username { get; init; }
    
    [StringLength(2056, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string AvatarUrl { get; init; }
    
    [StringLength(512, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Email { get; init; }
    
    [StringLength(1028, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string AccessToken { get; init; }
    
    public required List<LoginRecordEntity> LoginRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}