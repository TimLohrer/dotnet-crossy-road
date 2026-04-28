namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class SingleStreetLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Street, seed, zPosition, 0)
{
    protected override void GenerateElements() {}
}