using System.ComponentModel.DataAnnotations;
using Domain.OAuth;

namespace Database.Entity;

public abstract class ClientInfo : IClientInfo
{
    [StringLength(39, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string Ip { get; init; }
    
    [StringLength(2056, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public required string UserAgent { get; init; }
}