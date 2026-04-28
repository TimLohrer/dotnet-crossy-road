using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetTopLane(int zPosition) : CrossyMapLane(CrossyModelPart.STREET_TOP, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}