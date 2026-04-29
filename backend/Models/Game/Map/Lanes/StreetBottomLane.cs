namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetBottomLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.StreetBottom, seed, zPosition, 0)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Street;
    protected override void GenerateElements() {}
}