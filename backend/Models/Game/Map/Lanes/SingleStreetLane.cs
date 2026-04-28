using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class SingleStreetLane(int zPosition) : CrossyMapLane(CrossyModelPart.Street, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}