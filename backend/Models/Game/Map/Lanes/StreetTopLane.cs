namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetTopLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.StreetTop, seed, zPosition, 0)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Street;
    protected override void GenerateElements() {}
}