namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetMiddleLane(int zPosition) : CrossyMapLane(CrossyModelPart.STREET_MIDDLE, zPosition, 0)
{
    public override List<CrossyMapElement> GenerateElements() => [];
}