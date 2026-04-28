namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Water, seed, zPosition, -PixelSize)
{
    protected override void GenerateElements() {}
}