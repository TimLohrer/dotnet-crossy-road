using CrossyRoadApi.Models.Game.Map.Elements;

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

    private static readonly int CarCount = 5;

    private static readonly List<CrossyModelPart> AvailableCars =
    [
        CrossyModelPart.Car0, CrossyModelPart.Car0, CrossyModelPart.Car1, CrossyModelPart.Car1,
        CrossyModelPart.Car2, CrossyModelPart.Car2, CrossyModelPart.Car3
    ];

    public override CrossyModelPart LaneType => CrossyModelPart.Street;

    protected override void GenerateElements()
    {
        var startX = new List<int> { 13, -13 }[Randomizer.Next(2)]; // Randomizes direction
        var baseSpeed = Randomizer.Next(3, 10) / 5f; // Number between 1 and 3
        var zBasedSpeedMultiplyer = (float)Math.Min(1.5, zPosition / 900f + 1.0f);
        var speed = baseSpeed * zBasedSpeedMultiplyer;
        for (var i = 0; i < CarCount; i++)
        {
            var type = AvailableCars[Randomizer.Next(AvailableCars.Count)];
            var offset =
                (Elements.Count > 0
                    ? ((CrossyMovingMapElement)Elements.Last()).XOffset + Elements.Last().ModelWidth
                    : 0) + Randomizer.Next(2, 6);
            switch (type)
            {
                case CrossyModelPart.Car0:
                    AddElement(new Car0(this, startX, speed, offset));
                    break;
                case CrossyModelPart.Car1:
                    AddElement(new Car1(this, startX, speed, offset));
                    break;
                case CrossyModelPart.Car2:
                    AddElement(new Car2(this, startX, speed, offset));
                    break;
                case CrossyModelPart.Car3:
                    AddElement(new Car3(this, startX, speed, offset));
                    break;
            }
        }
    }
}