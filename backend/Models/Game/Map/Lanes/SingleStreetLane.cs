using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class SingleStreetLane(int zPosition) : CrossyMapLane(CrossyModelPart.STREET, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}