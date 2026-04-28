using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map.Lanes;

public class PlainsLane(int zPosition, int seed) : CrossyMapLane(CrossyModelPart.Plains, seed, zPosition, PixelSize)
{
    private static List<CrossyModelPart> AvailableElements = [CrossyModelPart.Stone_0, CrossyModelPart.Stone_1, CrossyModelPart.Tree_0, CrossyModelPart.Tree_1, CrossyModelPart.Tree_2, CrossyModelPart.Tree_3, CrossyModelPart.Tree_4];
    
    protected override void GenerateElements()
    {
        var blockedSlots = GenerateBlockedSlotsLocations();
        foreach (var blockedSlot in blockedSlots)
        {
            var elementIndex = Randomizer.Next(0, AvailableElements.Count);
            var elementType = AvailableElements[elementIndex];
            switch (elementType)
            {
                case CrossyModelPart.Stone_1:
                    AddElement(new Stone1(this, blockedSlot));
                    break;
                case CrossyModelPart.Stone_0:
                    AddElement(new Stone0(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree_0:
                    AddElement(new Tree0(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree_1:
                    AddElement(new Tree1(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree_2:
                    AddElement(new Tree2(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree_3:
                    AddElement(new Tree3(this, blockedSlot));
                    break;
                case CrossyModelPart.Tree_4:
                    AddElement(new Tree4(this, blockedSlot));
                    break;
                default:
                    return;
            }
        }
    }
    
    protected List<int> GenerateBlockedSlotsLocations()
    {
        string binary;

        do
        {
            int value = Randomizer.Next(0, 1 << 25);
            binary = Convert.ToString(value, 2).PadLeft(24, '0');
        }
        // at least 9 of the inner 12 locations have to be empty
        while (binary.Substring(6, 12).Count(c => c == '0') < 9);
        
        var locations = new List<int>();
        for (var i = 0; i < binary.Length; i++)
        {
            if (binary[i] == '1')
            {
                locations.Add(GetMapPositionFromLanePositionIndex(i));
            }
        }
        return locations;
    }
}