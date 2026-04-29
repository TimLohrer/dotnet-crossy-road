using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class PlainsLane(PlainsLane.PlainsType type, int zPosition, int seed) : CrossyMapLane(
    type == PlainsType.Light ? CrossyModelPart.Plains : CrossyModelPart.PlainsDark, seed, zPosition, PixelSize)
{
    public enum PlainsType
    {
        Light,
        Dark
    }

    private static readonly int LeftMask = 0b1111111000000000000000000;
    private static readonly int MiddleMask = 0b0000000111111111111000000;
    private static readonly int RightMask = 0b0000000000000000000111111;

    private static readonly List<CrossyModelPart> LeftElements =
        [CrossyModelPart.Tree1, CrossyModelPart.Tree2, CrossyModelPart.Tree3, CrossyModelPart.Tree4];

    private static readonly List<CrossyModelPart> MiddleElements =
    [
        CrossyModelPart.Stone0, CrossyModelPart.Stone1, CrossyModelPart.Tree0, CrossyModelPart.Tree1,
        CrossyModelPart.Tree2, CrossyModelPart.Tree3, CrossyModelPart.Tree4
    ];

    private static readonly List<CrossyModelPart> RightElements =
        [CrossyModelPart.Tree1, CrossyModelPart.Tree2, CrossyModelPart.Tree3, CrossyModelPart.Tree4];

    public override CrossyModelPart LaneType => CrossyModelPart.Plains;

    protected override void GenerateElements()
    {
        var blockedSlots = GenerateBlockedSlots();
        for (var i = 0; i < blockedSlots.Count; i++)
        {
            var mapX = GetMapPositionFromLanePositionIndex(i);
            switch (blockedSlots[i])
            {
                case CrossyModelPart.Stone1:
                    AddElement(new Stone1(this, mapX));
                    break;
                case CrossyModelPart.Stone0:
                    AddElement(new Stone0(this, mapX));
                    break;
                case CrossyModelPart.Tree0:
                    AddElement(new Tree0(this, mapX));
                    break;
                case CrossyModelPart.Tree1:
                    AddElement(new Tree1(this, mapX));
                    break;
                case CrossyModelPart.Tree2:
                    AddElement(new Tree2(this, mapX));
                    break;
                case CrossyModelPart.Tree3:
                    AddElement(new Tree3(this, mapX));
                    break;
                case CrossyModelPart.Tree4:
                    AddElement(new Tree4(this, mapX));
                    break;
                default:
                    continue;
            }
        }
    }

    private List<CrossyModelPart> GenerateBlockedSlots()
    {
        var left = GenerateBlockedSlotsWithConstraints(LeftMask, 5, 7, LeftElements);
        var middle = GenerateBlockedSlotsWithConstraints(MiddleMask, 0, 3, MiddleElements);
        var right = GenerateBlockedSlotsWithConstraints(RightMask, 5, 6, RightElements);

        // Combine generated sections (numbers already represent the model part enum)
        // Input:  0243951 000000000000 000000
        //         0000000 340050080000 000000
        //         0000000 000000000000 285609
        // Output: 0243951 340050080000 285609
        List<CrossyModelPart> blockedSlots = [];
        for (var i = 0; i < LaneLength; i++)
            blockedSlots.Add(left[i] != CrossyModelPart.Empty ? left[i] :
                middle[i] != CrossyModelPart.Empty ? middle[i] : right[i]);

        // Console.WriteLine($"Map: {string.Join(" ", blockedSlots.Select(b => b.ToString()))}");

        return blockedSlots;
    }

    private List<CrossyModelPart> GenerateBlockedSlotsWithConstraints(int mask, int minFlags, int maxFlags,
        List<CrossyModelPart> availableParts)
    {
        minFlags = Math.Max(minFlags, 0);
        minFlags = Math.Min(minFlags, LaneLength);
        maxFlags = Math.Max(maxFlags, 0);
        maxFlags = Math.Min(maxFlags, LaneLength);
        if (availableParts.Count < 1 || availableParts.Count > 9)
            throw new IndexOutOfRangeException("'availableParts' has to be between 1 and 9 long");

        bool IsBinaryWithinContraints(string binary, int minFlags, int maxFlags)
        {
            var flagsRegion = string.Join("", binary.Where(c => c == '1').ToList());
            var hasMinFlags = flagsRegion.Length >= minFlags;
            var hasMaxFlags = flagsRegion.Length <= maxFlags;

            return hasMinFlags && hasMaxFlags;
        }

        string binary;
        do
        {
            var value = Randomizer.Next(0, 1 << LaneLength);
            // Mask 25 bits to only generate a section of the map
            // Binary: 0101101 110010010000 101011
            // Mask:   0000000 111111111111 000000
            // Result: 0000000 110010010000 000000
            // => Only generate the middle section of the map
            value &= mask;
            binary = Convert.ToString(value, 2).PadLeft(LaneLength, '0');
        } while (!IsBinaryWithinContraints(binary, minFlags, maxFlags));

        // Set indexes of available elements to occupied slots in the generated map
        // Input:  0000000 110010010000 000000
        // Output: 0000000 340050080000 000000
        // => Fill parts of the map with different sets of elements
        List<CrossyModelPart> blockedSlots = [];
        for (var i = 0; i < binary.Length; i++)
            if (binary[i] == '1')
            {
                var modelPartIndex = Randomizer.Next(1, availableParts.Count) - 1;
                blockedSlots.Add(availableParts[modelPartIndex]);
            }
            else
            {
                blockedSlots.Add(CrossyModelPart.Empty);
            }

        // Console.WriteLine($"Mask: {Convert.ToString(mask, 2).PadLeft(LaneLength, '0')}");
        // Console.WriteLine($"Binary: {binary}");
        // Console.WriteLine($"Map Section: {string.Join("", blockedSlots.Select(b => availableParts.IndexOf(b) + 1))}");

        return blockedSlots;
    }
}