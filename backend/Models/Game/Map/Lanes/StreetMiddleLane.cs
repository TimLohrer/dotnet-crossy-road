namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetMiddleLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.StreetMiddle, seed, zPosition, 0)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Street;
    protected override void GenerateElements() {}
}