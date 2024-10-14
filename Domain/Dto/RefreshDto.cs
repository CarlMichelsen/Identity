using System.Text.Json.Serialization;

namespace Domain.Dto;

public record RefreshDto(
    [property: JsonPropertyName("refreshId")] long RefreshId,
    [property: JsonPropertyName("accessRecords")] List<AccessDto> AccessRecords,
    [property: JsonPropertyName("expiresUtc")] DateTime ExpiresUtc,
    [property: JsonPropertyName("createdUtc")] DateTime CreatedUtc,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("userAgent")] string UserAgent,
    [property: JsonPropertyName("invalidatedUtc")] DateTime? InvalidatedUtc = default);