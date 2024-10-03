using System.ComponentModel.DataAnnotations;

namespace Database.Entity;

public abstract class ClientInfo
{
    [StringLength(39, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Ip { get; init; }
    
    [StringLength(2056, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string UserAgent { get; init; }
}