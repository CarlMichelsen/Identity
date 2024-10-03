namespace Database.Entity;

public class LoginRecordEntity : ClientInfo
{
    public required long Id { get; set; }
    
    public required long UserId { get; init; }
    
    public required UserEntity User { get; init; }
    
    public required List<RefreshRecordEntity> RefreshRecords { get; init; }
    
    public required DateTime CreatedUtc { get; init; }
}