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

    private static readonly int CarCount = 20;
    private static readonly List<int> AvailableDelays = [1000, 1000, 1000, 1500, 1500, 2000, 2500, 3000];

    private static readonly List<CrossyModelPart> AvailableCars =
    [
        CrossyModelPart.Car0, CrossyModelPart.Car0, CrossyModelPart.Car0, CrossyModelPart.Car1, CrossyModelPart.Car1,
        CrossyModelPart.Car2, CrossyModelPart.Car2, CrossyModelPart.Car3
    ];

    public override CrossyModelPart LaneType => CrossyModelPart.Street;

    protected override void GenerateElements()
    {
        var startX = new List<int> { 12, -12 }[Randomizer.Next(2)];
        var baseSpeed = Randomizer.Next(1, 4) / 10; // Number between 0.1 and 0.3
        var zBasedSpeedMultiplyer = zPosition / 1000 + 1; // If score is e.g. 100 -> 1.1
        var speed = baseSpeed * zBasedSpeedMultiplyer;
        for (var i = 0; i < CarCount; i++)
        {
            var type = AvailableCars[Randomizer.Next(AvailableCars.Count)];
            var delay = AvailableDelays[Randomizer.Next(AvailableDelays.Count)];
            switch (type)
            {
                case CrossyModelPart.Car0:
                    AddElement(new Car0(this, startX, speed, delay));
                    break;
                case CrossyModelPart.Car1:
                    AddElement(new Car1(this, startX, speed, delay * 2));
                    break;
                case CrossyModelPart.Car2:
                    AddElement(new Car2(this, startX, speed, delay * 2));
                    break;
                case CrossyModelPart.Car3:
                    AddElement(new Car3(this, startX, speed, delay * 2));
                    break;
            }
        }
    }
}