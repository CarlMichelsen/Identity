using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.JsonConverters;

public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTime = reader.GetDateTime();
        
        // If it's already UTC, return as-is
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime;
        }
        
        // If unspecified, assume UTC
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            throw new JsonException("DateTimeKind.Unspecified is not supported");
        }

        // If local, convert to UTC
        return dateTime.ToUniversalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("O")); // ISO 8601 format
    }
}