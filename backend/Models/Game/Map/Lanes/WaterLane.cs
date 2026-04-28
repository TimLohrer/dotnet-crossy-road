using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition) : CrossyMapLane(CrossyModelPart.WATER, zPosition, -PixelSize)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}