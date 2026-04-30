using CrossyRoadApi.Models.Game.Map.Lanes;
using CrossyRoadApi.Utils;

namespace CrossyRoadApi.Models.Game.Map;

public static class CrossyMapGenerator
{
    private static readonly List<CrossyModelPart> AvailableLanes =
        [CrossyModelPart.Plains, CrossyModelPart.Street, CrossyModelPart.Water];

    private static readonly int MaxPlainsLength = 3;
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
                lanes = GeneratePlainsLanes(randomizer, seed, zPosition);
                break;
            case CrossyModelPart.Street:
                lanes = GenerateStreetLanes(randomizer, seed, zPosition);
                break;
            case CrossyModelPart.Water:
                lanes = GenerateWaterLanes(randomizer, seed, zPosition);
                break;
            default:
                lanes = GeneratePlainsLanes(randomizer, seed, zPosition);
                break;
        }

        return lanes;
    }

    private static List<CrossyMapLane> GeneratePlainsLanes(Random randomizer, int seed, int zPosition)
    {
        var plainsLength = randomizer.Next(1, MaxPlainsLength + 1);

        List<CrossyMapLane> lanes = [];
        for (var i = 0; i < plainsLength; i++)
        {
            var type = i % 2 == 0 ? PlainsLane.PlainsType.Light : PlainsLane.PlainsType.Dark;
            lanes.Add(new PlainsLane(type, zPosition + i, seed));
        }

        return lanes;
    }

    private static List<CrossyMapLane> GenerateStreetLanes(Random randomizer, int seed, int zPosition)
    {
        var streetLength = randomizer.Next(1, MaxStreetLength + 1);

        List<CrossyMapLane> lanes = [];
        if (streetLength == 1)
            lanes.Add(new StreetLane(StreetLane.StreetType.Single, zPosition, seed));
        else
            for (var i = 0; i < streetLength; i++)
                if (i == 0)
                    lanes.Add(new StreetLane(StreetLane.StreetType.Bottom, zPosition, seed));
                else if (i == streetLength - 1)
                    lanes.Add(new StreetLane(StreetLane.StreetType.Top, zPosition + i, seed));
                else
                    lanes.Add(new StreetLane(StreetLane.StreetType.Middle, zPosition + i, seed));

        return lanes;
    }

    private static List<CrossyMapLane> GenerateWaterLanes(Random randomizer, int seed, int zPosition)
    {
        var waterLength = randomizer.Next(1, MaxWaterLenth + 1);

        List<CrossyMapLane> lanes = [];
        for (var i = 0; i < waterLength; i++) lanes.Add(new WaterLane(zPosition + i, seed));

        return lanes;
    }

    public static List<CrossyMapLane> GenerateMapStart(int seed)
    {
        List<CrossyMapLane> lanes = [];
        for (var i = -1; i > -10; i--)
            lanes.Add(new PlainsLane(i % 2 == 0 ? PlainsLane.PlainsType.Light : PlainsLane.PlainsType.Dark, i, 0,
                true));
        for (var i = 0; i < 15;)
        {
            var section = CrossyMapGenerator.GenerateMapSection(seed, i);
            lanes.AddRange(section);
            i += section.Count;
        }
        return lanes;
    }
}