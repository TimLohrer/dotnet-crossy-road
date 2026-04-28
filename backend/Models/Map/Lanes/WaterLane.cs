using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class WaterLane : CrossyMapLane
{
    public WaterLane(Vector3 position) : base(CrossyModelPart.WATER, position, -PixelSize)
    {
    }
    
    public override List<CrossyMapElement> GenerateElements() => [];
}