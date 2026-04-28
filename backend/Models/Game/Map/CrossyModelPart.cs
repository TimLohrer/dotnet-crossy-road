using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CrossyRoadApi.Models.Game.Map;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CrossyModelPart
{
    [JsonPropertyName("plains")]
    Plains,
    [JsonPropertyName("water")]
    Water,
    [JsonPropertyName("street")]
    Street,
    [JsonPropertyName("street_bottom")]
    StreetBottom,
    [JsonPropertyName("street_middle")]
    StreetMiddle,
    [JsonPropertyName("street_top")]
    StreetTop,
    
    [JsonPropertyName("tree_0")]
    Tree0,
    [JsonPropertyName("tree_1")]
    Tree1,
    [JsonPropertyName("tree_2")]
    Tree2,
    [JsonPropertyName("tree_3")]
    Tree3,
    [JsonPropertyName("tree_4")]
    Tree4,
    
    [JsonPropertyName("stone_0")]
    Stone0,
    [JsonPropertyName("stone_1")]
    Stone1,
    
    [JsonPropertyName("log_0")]
    Log0
}