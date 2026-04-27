using System.ComponentModel;

namespace CrossyRoadApi.Map;

public enum CrossyModelPart
{
    [Description("plains")]
    PLAINS,
    [Description("water")]
    WATER,
    [Description("street")]
    STREET,
    [Description("street_start")]
    STREET_START,
    [Description("street_middle")]
    STREET_MIDDLE,
    [Description("street_end")]
    STREET_END,
    
    [Description("tree_0")]
    TREE_0,
    [Description("tree_1")]
    TREE_1,
    [Description("tree_2")]
    TREE_2,
    [Description("tree_3")]
    TREE_3,
    [Description("tree_4")]
    TREE_4,
    
    [Description("stone_0")]
    STONE_0,
    [Description("stone_1")]
    STONE_1,
    
    [Description("log_0")]
    LOG_0
}