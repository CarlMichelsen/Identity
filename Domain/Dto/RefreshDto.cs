namespace Domain.Dto;

public record RefreshDto(
    long RefreshId,
    List<AccessDto> AccessRecords,
    DateTime ExpiresUtc,
    DateTime CreatedUtc,
    string Ip,
    string UserAgent,
    DateTime? InvalidatedUtc = default);