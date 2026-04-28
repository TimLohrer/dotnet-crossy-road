namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetMiddleLane(int zPosition) : CrossyMapLane(CrossyModelPart.StreetMiddle, zPosition, 0)
{
    public override void GenerateElements(int seed) {}
}