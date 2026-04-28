namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetBottomLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.StreetBottom, seed, zPosition, 0)
{
    protected override void GenerateElements() {}
}