using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class RailLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Rail, seed, zPosition)
{
    public override CrossyModelPart LaneType => CrossyModelPart.Rail;

    protected override void GenerateElements()
    {
        var startX = new List<int> { 13, -13 }[Randomizer.Next(2)]; // Randomizes direction
        var offset = Randomizer.Next(400, 1000);
        Elements.Add(new Train(this, startX, offset));
    }
}