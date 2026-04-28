using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class StreetMiddleLane : CrossyMapLane
{
    public StreetMiddleLane(Vector3 position) : base(CrossyModelPart.STREET_MIDDLE, position, 0)
    {
    }
    
    public override List<CrossyMapElement> GenerateElements() => [];
}