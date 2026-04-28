using System.Text.Json.Serialization;

namespace CrossyRoadApi.Models.Game.Map;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CrossyModelPart
{
    [JsonPropertyName("PLAINS")]
    Plains,
    [JsonPropertyName("WATER")]
    Water,
    [JsonPropertyName("STREET")]
    Street,
    [JsonPropertyName("STREET_BOTTOM")]
    StreetBottom,
    [JsonPropertyName("STREET_MIDDLE")]
    StreetMiddle,
    [JsonPropertyName("STREET_TOP")]
    StreetTop,
    
    [JsonPropertyName("TREE_0")]
    Tree0,
    [JsonPropertyName("TREE_1")]
    Tree1,
    [JsonPropertyName("TREE_2")]
    Tree2,
    [JsonPropertyName("TREE_3")]
    Tree3,
    [JsonPropertyName("TREE_4")]
    Tree4,
    
    [JsonPropertyName("STONE_0")]
    Stone0,
    [JsonPropertyName("STONE_1")]
    Stone1,
    
    [JsonPropertyName("LOG_0")]
    Log0
}