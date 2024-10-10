namespace Domain.Dto;

public record AccessDto(
    long AccessId,
    DateTime ExpiresUtc,
    DateTime CreatedUtc,
    string Ip,
    string UserAgent);