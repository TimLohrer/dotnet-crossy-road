using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class PlainsLane(int zPosition) : CrossyMapLane(CrossyModelPart.PLAINS, zPosition, PixelSize)
{
    public override List<CrossyMapElement> GenerateElements()
    {
        return [];
    }
}