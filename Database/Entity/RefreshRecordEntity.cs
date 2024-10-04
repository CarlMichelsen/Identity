namespace Database.Entity;

public class RefreshRecordEntity : ClientInfo
{
    public long Id { get; set; }
    
    public required long LoginRecordId { get; init; }
    
    public required LoginRecordEntity LoginRecord { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}