using CrossyRoadApi.Models.Game.Map.Lanes;
using CrossyRoadApi.Utils;

namespace CrossyRoadApi.Models.Game.Map;

public static class CrossyMapGenerator
{
    private static readonly List<CrossyModelPart> AvailableLanes = [CrossyModelPart.Plains, CrossyModelPart.Street, CrossyModelPart.Water];
    private static readonly int MaxStreetLength = 4;
    private static readonly int MaxWaterLenth = 4;
    // TODO: Move this into game instance later
    private static CrossyModelPart lastSectionType;
    
    public static List<CrossyMapLane> GenerateMapSection(int seed, int zPosition)
    {
        var randomizer = CrossyRandomizer.Get(seed, zPosition);
        
        CrossyModelPart laneType;
        do
        {
            laneType = AvailableLanes[randomizer.Next(0, AvailableLanes.Count)];
        } while (laneType == lastSectionType);
        
        lastSectionType = laneType;
        
        List<CrossyMapLane> lanes;
        switch (laneType)
        {
            case CrossyModelPart.Plains:
                lanes = [new PlainsLane(zPosition, seed)];
                break;
            case CrossyModelPart.Street:
                lanes = GenerateStreetLanes(randomizer, seed, zPosition);
                break;
            case CrossyModelPart.Water:
                lanes = GenerateWaterLanes(randomizer, seed, zPosition);
                break;
            default:
                lanes = [new PlainsLane(zPosition, seed)];
                break;
        }
        
        return lanes;
    }

    private static List<CrossyMapLane> GenerateStreetLanes(Random randomizer, int seed, int zPosition)
    {
        var streetLength = randomizer.Next(1, MaxStreetLength + 1);

        List<CrossyMapLane> lanes = [];
        if (streetLength == 1)
        {
            lanes.Add(new SingleStreetLane(zPosition, seed));
        }
        else
        {
            for (var i = 0; i < streetLength; i++)
            {
                if (i == 0)
                {
                    lanes.Add(new StreetBottomLane(zPosition, seed));
                }
                else if (i == streetLength - 1)
                {
                    lanes.Add(new StreetTopLane(zPosition + i, seed));
                }
                else
                {
                    lanes.Add(new StreetMiddleLane(zPosition + i, seed));
                }
            }
        }
        
        return lanes;
    }

    private static List<CrossyMapLane> GenerateWaterLanes(Random randomizer, int seed, int zPosition)
    {
        var waterLength = randomizer.Next(1, MaxWaterLenth + 1);

        List<CrossyMapLane> lanes = [];
        for (var i = 0; i < waterLength; i++)
        {
            lanes.Add(new WaterLane(zPosition + i, seed));
        }
        
        return lanes;
    }
}