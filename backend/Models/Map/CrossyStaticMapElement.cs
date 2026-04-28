using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public abstract class CrossyStaticMapElement : CrossyMapElement
{
    public CrossyStaticMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, int modelWidth) : base(modelPart, lane, position, modelWidth)
    {
        
    }
}