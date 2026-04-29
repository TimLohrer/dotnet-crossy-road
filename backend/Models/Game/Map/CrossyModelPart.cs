using System.Runtime.Serialization;

namespace CrossyRoadApi.Models.Game.Map;

public enum CrossyModelPart
{
    [EnumMember(Value = "empty")]
    Empty,
    
    [EnumMember(Value = "plains")]
    Plains,
    [EnumMember(Value = "water")]
    Water,
    [EnumMember(Value = "street")]
    Street,
    [EnumMember(Value = "street_bottom")]
    StreetBottom,
    [EnumMember(Value = "street_middle")]
    StreetMiddle,
    [EnumMember(Value = "street_top")]
    StreetTop,
    
    [EnumMember(Value = "tree_0")]
    Tree0,
    [EnumMember(Value = "tree_1")]
    Tree1,
    [EnumMember(Value = "tree_2")]
    Tree2,
    [EnumMember(Value = "tree_3")]
    Tree3,
    [EnumMember(Value = "tree_4")]
    Tree4,
    
    [EnumMember(Value = "stone_0")]
    Stone0,
    [EnumMember(Value = "stone_1")]
    Stone1,
    
    [EnumMember(Value = "log_0")]
    Log0
}