using System.ComponentModel.DataAnnotations;

namespace Database.Entity;

public class AccessRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long RefreshRecordId { get; init; }
    
    public required RefreshRecordEntity RefreshRecord { get; init; }
    
    public required DateTime ExpiresUtc { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}