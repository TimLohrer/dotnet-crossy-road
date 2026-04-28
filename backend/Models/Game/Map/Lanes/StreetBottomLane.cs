using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetBottomLane(int zPosition) : CrossyMapLane(CrossyModelPart.STREET_BOTTOM, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}