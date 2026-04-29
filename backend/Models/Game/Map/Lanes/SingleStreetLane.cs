namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class SingleStreetLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Street, seed, zPosition, 0)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Street;
    protected override void GenerateElements() {}
}