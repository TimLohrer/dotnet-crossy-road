using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public abstract class CrossyMovingMapElement : CrossyMapElement
{
    public CrossyMovingMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, int modelWidth) : base(modelPart, lane, position, modelWidth)
    {
        
    }
}