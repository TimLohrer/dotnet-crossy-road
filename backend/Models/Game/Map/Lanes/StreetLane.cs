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
    private const int LaneLoopWidth = 30;
    private const int MinGapBetweenCars = 2;
    private const int MaxGapBetweenCars = 6;

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

        int? firstOffset = null;
        for (var i = 0; i < CarCount; i++)
        {
            var type = AvailableCars[Randomizer.Next(AvailableCars.Count)];
            var modelWidth = GetModelWidth(type);
            if (!TryGetNextOffset(modelWidth, firstOffset, out var offset))
                break;

            firstOffset ??= offset;
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
        GenerateTalers(0);
    }

    private bool TryGetNextOffset(int modelWidth, int? firstOffset, out int offset)
    {
        var previousEnd = Elements.Count > 0
            ? ((CrossyMovingMapElement)Elements.Last()).XOffset + Elements.Last().ModelWidth
            : 0;

        var minOffset = (Elements.Count == 0 ? 0 : previousEnd) + MinGapBetweenCars;
        var maxOffset = (Elements.Count == 0 ? 0 : previousEnd) + MaxGapBetweenCars;

        if (firstOffset.HasValue)
        {
            // Keep a minimum wrap gap so the first and last car don't overlap
            // when both are modulo-wrapped on the client.
            var maxOffsetByWrap = LaneLoopWidth - modelWidth - MinGapBetweenCars + firstOffset.Value;
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
            CrossyModelPart.Car0 => 1,
            CrossyModelPart.Car1 => 2,
            CrossyModelPart.Car2 => 2,
            CrossyModelPart.Car3 => 3,
            _ => 1
        };
    }
}