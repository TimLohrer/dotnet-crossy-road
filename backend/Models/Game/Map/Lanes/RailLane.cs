namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class RailLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Rail, seed, zPosition)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Rail;

    protected override void GenerateElements()
    {
    }
}