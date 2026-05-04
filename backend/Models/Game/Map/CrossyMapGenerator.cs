using CrossyRoadApi.Models.Game.Map.Lanes;
using CrossyRoadApi.Utils;

namespace CrossyRoadApi.Models.Game.Map;

public class CrossyMapGenerator(CrossyWsGame wsGame)
{
    private static readonly List<CrossyModelPart> AvailableLanes =
        [CrossyModelPart.Plains, CrossyModelPart.Street, CrossyModelPart.Water, CrossyModelPart.Rail];

    private static readonly int MaxPlainsLength = 5;
    private static readonly int MaxStreetLength = 6;
    private static readonly int MaxWaterLenth = 5;
    private static readonly int MaxRailLenth = 4;

    private readonly int _seed = wsGame.Seed;

    public List<CrossyMapLane> GenerateMapSection(int zPosition, CrossyWsPlayer wsPlayer)
    {
        var randomizer = CrossyRandomizer.Get(_seed, zPosition);

        CrossyModelPart laneType;
        do
        {
            laneType = AvailableLanes[randomizer.Next(0, AvailableLanes.Count)];
        } while (laneType == wsPlayer.LastGeneratedSectionType);

        wsPlayer.LastGeneratedSectionType = laneType;

        List<CrossyMapLane> lanes;
        switch (laneType)
        {
            case CrossyModelPart.Plains:
                lanes = GeneratePlainsLanes(randomizer, _seed, zPosition);
                break;
            case CrossyModelPart.Street:
                lanes = GenerateStreetLanes(randomizer, _seed, zPosition);
                break;
            case CrossyModelPart.Water:
                lanes = GenerateWaterLanes(randomizer, _seed, zPosition);
                break;
            case CrossyModelPart.Rail:
                lanes = GenerateRailLanes(randomizer, _seed, zPosition);
                break;
            default:
                lanes = GeneratePlainsLanes(randomizer, _seed, zPosition);
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

    private static List<CrossyMapLane> GenerateRailLanes(Random randomizer, int seed, int zPosition)
    {
        var waterLength = randomizer.Next(1, MaxRailLenth + 1);

        List<CrossyMapLane> lanes = [];
        for (var i = 0; i < waterLength; i++) lanes.Add(new RailLane(zPosition + i, seed));

        return lanes;
    }

    public List<CrossyMapLane> GenerateMapStart(CrossyWsPlayer wsPlayer)
    {
        List<CrossyMapLane> lanes = [];
        for (var i = -10; i < 0; i++)
            lanes.Add(new PlainsLane(i % 2 == 0 ? PlainsLane.PlainsType.Light : PlainsLane.PlainsType.Dark, i, 0,
                true));
        for (var i = 0; i < 15;)
        {
            var section = GenerateMapSection(i, wsPlayer);
            lanes.AddRange(section);
            i += section.Count;
        }

        return lanes;
    }
}