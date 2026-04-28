using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public class CrossyStaticMapElement : CrossyMapElement
{
    public CrossyStaticMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, CrossyTheme? theme) : base(modelPart, lane, position,
        theme)
    {
        
    }
}