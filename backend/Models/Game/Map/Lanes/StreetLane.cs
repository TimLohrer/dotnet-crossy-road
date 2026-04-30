namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class StreetLane(StreetLane.StreetType type, int zPosition, int seed) : CrossyMapLane(
    type == StreetType.Single ? CrossyModelPart.Street :
    type == StreetType.Top ? CrossyModelPart.StreetTop :
    type == StreetType.Middle ? CrossyModelPart.StreetMiddle : CrossyModelPart.StreetBottom, seed, zPosition)
{
    public enum StreetType
    {
        Single,
        Top,
        Middle,
        Bottom
    }

    public override CrossyModelPart LaneType => CrossyModelPart.Street;

    protected override void GenerateElements()
    {
    }
}