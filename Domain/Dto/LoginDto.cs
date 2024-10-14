using System.Text.Json.Serialization;

namespace Domain.Dto;

public record LoginDto(
    [property: JsonPropertyName("loginId")] long LoginId,
    [property: JsonPropertyName("refreshRecords")] List<RefreshDto> RefreshRecords,
    [property: JsonPropertyName("createdUtc")] DateTime CreatedUtc,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("userAgent")] string UserAgent,
    [property: JsonPropertyName("invalidatedUtc")] DateTime? InvalidatedUtc = default);