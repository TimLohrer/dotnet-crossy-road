using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class SingleStreetLane : CrossyMapLane
{
    public SingleStreetLane(Vector3 position) : base(CrossyModelPart.STREET, position, 0)
    {
    }
    
    public override List<CrossyMapElement> GenerateElements() => [];
}