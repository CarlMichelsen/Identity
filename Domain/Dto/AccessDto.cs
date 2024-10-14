using System.Text.Json.Serialization;

namespace Domain.Dto;

public record AccessDto(
    [property: JsonPropertyName("accessId")] long AccessId,
    [property: JsonPropertyName("expiresUtc")] DateTime ExpiresUtc,
    [property: JsonPropertyName("createdUtc")] DateTime CreatedUtc,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("userAgent")] string UserAgent);