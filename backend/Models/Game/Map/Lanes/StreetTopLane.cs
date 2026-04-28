using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetTopLane(int zPosition) : CrossyMapLane(CrossyModelPart.StreetTop, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}