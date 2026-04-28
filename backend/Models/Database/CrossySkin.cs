using System.Text.Json.Serialization;

namespace CrossyRoadApi.Models.Database;

[JsonConverter(typeof(JsonPropertyNameAttribute))]
public enum CrossySkin
{
    [JsonPropertyName("DEFAULT")]
    Default
}