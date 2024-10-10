namespace Domain.Dto;

public record LoginDto(
    long LoginId,
    List<RefreshDto> RefreshRecords,
    DateTime CreatedUtc,
    string Ip,
    string UserAgent,
    DateTime? InvalidatedUtc = default);