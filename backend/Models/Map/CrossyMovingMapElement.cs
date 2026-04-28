using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public class CrossyMovingMapElement : CrossyMapElement
{
    public CrossyMovingMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, CrossyTheme? theme) : base(modelPart, lane, position,
        theme)
    {
        
    }
}