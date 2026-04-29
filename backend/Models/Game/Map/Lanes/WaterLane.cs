namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Water, seed, zPosition, -PixelSize)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Water;

    protected override void GenerateElements()
    {
    }
}