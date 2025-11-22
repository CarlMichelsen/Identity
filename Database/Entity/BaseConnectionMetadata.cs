using System.ComponentModel.DataAnnotations;

namespace Database.Entity;

public abstract class BaseConnectionMetadata
{
    [MinLength(4)]
    [MaxLength(64)]
    public required string RemoteIpAddress { get; init; }
    
    public required int RemotePort { get; init; }
    
    [MinLength(2)]
    [MaxLength(1028)]
    public required string? UserAgent { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}