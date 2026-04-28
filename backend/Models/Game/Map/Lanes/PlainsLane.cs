using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class PlainsLane(int zPosition) : CrossyMapLane(CrossyModelPart.Plains, zPosition, PixelSize)
{
    private static List<CrossyModelPart> AvailableElements = [CrossyModelPart.Stone0, CrossyModelPart.Stone1, CrossyModelPart.Tree0, CrossyModelPart.Tree1, CrossyModelPart.Tree2, CrossyModelPart.Tree3, CrossyModelPart.Tree4];
    
    public override void GenerateElements(int seed)
    {
        var random = new Random(seed);
        var blockedSlots = GenerateBlockedSlotsLocations(random);
        foreach (var blockedSlot in blockedSlots)
        {
            var elementIndex = random.Next(0, AvailableElements.Count);
            var elementType = AvailableElements[elementIndex];
            switch (elementType)
            {
                case CrossyModelPart.Stone1:
                    AddElement(new Stone1(this, blockedSlot));
                    break;
                case CrossyModelPart.Stone0:
                    AddElement(new Stone0(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree0:
                    AddElement(new Tree0(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree1:
                    AddElement(new Tree1(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree2:
                    AddElement(new Tree2(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree3:
                    AddElement(new Tree3(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree4:
                    AddElement(new Tree4(this, blockedSlot));
                    break;
                default:
                    return;
            }
        }
    }
    
    protected List<int> GenerateBlockedSlotsLocations(Random random)
    {
        string binary;

        do
        {
            int value = random.Next(0, 1 << 24);
            binary = Convert.ToString(value, 2).PadLeft(24, '0');
        }
        while (binary.Count(c => c == '0') < 5);
        
        var locations = new List<int>();
        for (var i = 0; i < binary.Length; i++)
        {
            if (binary[i] == '0')
            {
                locations.Add(GetMapPositionFromLanePositionIndex(i));
            }
        }
        return locations;
    }
}