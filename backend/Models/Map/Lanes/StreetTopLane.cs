using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class StreetTopLane : CrossyMapLane
{
    public StreetTopLane(Vector3 position) : base(CrossyModelPart.STREET_TOP, position, 0)
    {
    }
    
    public override List<CrossyMapElement> GenerateElements() => [];
}