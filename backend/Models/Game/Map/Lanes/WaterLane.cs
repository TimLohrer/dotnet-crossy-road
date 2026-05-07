using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition, int seed, bool canBeLillyLane)
    : CrossyMapLane(CrossyModelPart.Water, seed, zPosition)
{
    private static readonly int LogCount = 6;
    private static readonly int LillypadChance = 3; // 33% chance for a lillypad lane
    private static readonly int MaxLillypads = 3;

    private static readonly List<CrossyModelPart> AvailableLogs =
    [
        CrossyModelPart.Log0, CrossyModelPart.Log1, CrossyModelPart.Log1, CrossyModelPart.Car2
    ];

    public override CrossyModelPart LaneType => CrossyModelPart.Water;

    protected override void GenerateElements()
    {
        var isLillypadLane = Randomizer.Next(LillypadChance) == 0;
        if (canBeLillyLane && isLillypadLane)
        {
            var lillypadCount = Randomizer.Next(1, MaxLillypads + 1);
            for (var i = 0; i < lillypadCount; i++)
            {
                int xPos;
                do
                {
                    xPos = Randomizer.Next(-6, 7);
                } while
                    (Elements.Any(e =>
                         Math.Abs(e.Position.X - xPos) < 2)); // Ensure lillypads are not too close to each other

                AddElement(new Lillypad(this, xPos));
            }
        }
        else
        {
            var startX = new List<int> { 13, -13 }[Randomizer.Next(2)]; // Randomizes direction
            var baseSpeed = Randomizer.Next(3, 5) / 5f; // Number between 0.6 and 1.0
            var zBasedSpeedMultiplyer = (float)Math.Min(1.5, zPosition / 900f + 1.0f);
            var speed = baseSpeed * zBasedSpeedMultiplyer;
            for (var i = 0; i < LogCount; i++)
            {
                var type = AvailableLogs[Randomizer.Next(AvailableLogs.Count)];
                var offset =
                    (Elements.Count > 0
                        ? ((CrossyMovingMapElement)Elements.Last()).XOffset + Elements.Last().ModelWidth
                        : 0) + Randomizer.Next(2, 6) + 1;
                switch (type)
                {
                    case CrossyModelPart.Log0:
                        AddElement(new Log0(this, startX, speed, offset));
                        break;
                    case CrossyModelPart.Log1:
                        AddElement(new Log1(this, startX, speed, offset));
                        break;
                    case CrossyModelPart.Log2:
                        AddElement(new Log2(this, startX, speed, offset));
                        break;
                }
            }
        }
    }
}