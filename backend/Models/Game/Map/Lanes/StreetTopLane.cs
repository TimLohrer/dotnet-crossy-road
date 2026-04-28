namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetTopLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.StreetTop, seed, zPosition, 0)
{
    protected override void GenerateElements() {}
}