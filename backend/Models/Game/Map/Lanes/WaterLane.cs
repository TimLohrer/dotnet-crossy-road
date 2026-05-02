using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Water, seed, zPosition)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Water;

    protected override void GenerateElements()
    {
        AddElement(new Lillypad(this, 0));
    }
}