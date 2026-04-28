using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class StreetBottomLane : CrossyMapLane
{
    public StreetBottomLane(Vector3 position) : base(CrossyModelPart.STREET_BOTTOM, position, 0)
    {
    }
    
    public override List<CrossyMapElement> GenerateElements() => [];
}