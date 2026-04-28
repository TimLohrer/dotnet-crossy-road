namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetBottomLane(int zPosition) : CrossyMapLane(CrossyModelPart.StreetBottom, zPosition, 0)
{
    public override void GenerateElements(int seed) {}
}