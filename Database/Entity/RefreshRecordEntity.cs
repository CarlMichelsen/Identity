using System.ComponentModel.DataAnnotations;

namespace Database.Entity;

public class RefreshRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long LoginRecordId { get; init; }
    
    public required LoginRecordEntity LoginRecord { get; init; }
    
    public required DateTime ExpiresUtc { get; init; }
    
    public required List<AccessRecordEntity> AccessRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
    
    public DateTime? InvalidatedUtc { get; set; }
}