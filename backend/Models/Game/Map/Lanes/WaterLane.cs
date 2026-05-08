using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class WaterLane(int zPosition, int seed, bool canBeLillyLane, CrossyMovingMapElement.ModelDirection direction)
    : CrossyMapLane(CrossyModelPart.Water, seed, zPosition)
{
    private static readonly int LogCount = 5;
    private static readonly int LillypadChance = 3; // 33% chance for a lillypad lane
    private static readonly int MaxLillypads = 3;
    private const int LaneLoopWidth = 30;
    private const int MinGapBetweenLogs = 2;
    private const int MaxGapBetweenLogs = 5;

    private static readonly List<CrossyModelPart> AvailableLogs =
    [
        CrossyModelPart.Log0, CrossyModelPart.Log1, CrossyModelPart.Log1, CrossyModelPart.Log2
    ];

    public override CrossyModelPart LaneType => CrossyModelPart.Water;

    protected override void GenerateElements()
    {
        var isLillypadLane = Randomizer.Next(LillypadChance) == 0;
        if (canBeLillyLane && isLillypadLane)
        {
            var lillypadCount = Randomizer.Next(2, MaxLillypads + 1);
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
            var startX =
                new List<int> { 13, -13 }[
                    direction == CrossyMovingMapElement.ModelDirection.Right ? 0 : 1]; // Randomizes direction
            var baseSpeed = Randomizer.Next(3, 5) / 5f; // Number between 0.6 and 1.0
            var zBasedSpeedMultiplyer = (float)Math.Min(1.5, zPosition / 900f + 1.0f);
            var speed = baseSpeed * zBasedSpeedMultiplyer;

            int? firstOffset = null;
            for (var i = 0; i < LogCount; i++)
            {
                var type = AvailableLogs[Randomizer.Next(AvailableLogs.Count)];
                var modelWidth = GetModelWidth(type);
                if (!TryGetNextOffset(modelWidth, firstOffset, out var offset))
                    break;

                firstOffset ??= offset;
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

    private bool TryGetNextOffset(int modelWidth, int? firstOffset, out int offset)
    {
        var previousEnd = Elements.Count > 0
            ? ((CrossyMovingMapElement)Elements.Last()).XOffset + Elements.Last().ModelWidth
            : 0;

        var minOffset = (Elements.Count == 0 ? 0 : previousEnd) + MinGapBetweenLogs;
        var maxOffset = (Elements.Count == 0 ? 0 : previousEnd) + MaxGapBetweenLogs;

        if (firstOffset.HasValue)
        {
            // Keep a minimum wrap gap so the first and last log don't overlap
            // when both are modulo-wrapped on the client.
            var maxOffsetByWrap = LaneLoopWidth - modelWidth - MinGapBetweenLogs + firstOffset.Value;
            maxOffset = Math.Min(maxOffset, maxOffsetByWrap);
        }

        if (minOffset > maxOffset)
        {
            offset = 0;
            return false;
        }

        offset = Randomizer.Next(minOffset, maxOffset + 1);
        return true;
    }

    private static int GetModelWidth(CrossyModelPart type)
    {
        return type switch
        {
            CrossyModelPart.Log0 => 1,
            CrossyModelPart.Log1 => 2,
            CrossyModelPart.Log2 => 3,
            _ => 1
        };
    }
}