namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition) : CrossyMapLane(CrossyModelPart.Water, zPosition, -PixelSize)
{
    public override void GenerateElements(int seed) {}
}