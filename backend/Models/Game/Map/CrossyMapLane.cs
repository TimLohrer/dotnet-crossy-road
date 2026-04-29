using System.Numerics;
using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapLane : CrossyModel
{
    protected static int LaneLength = 25;
    public List<CrossyMapElement> Elements = [];
    protected Random Randomizer { get; }

    public CrossyMapLane(CrossyModelPart modelPart, int seed, int zPosition, float baseYOffset) : base(modelPart, new Vector3(0, 0, zPosition))
    {
        var zRandom = new Random(zPosition);
        var zRandomizer1 = zRandom.Next(1, 99);
        var zRandomizer2 = zRandom.Next(1, 99);
        var zBasedSeed = zRandomizer1 < seed ? seed / zRandomizer1 * zRandomizer2 : seed * zRandomizer2 + zRandomizer2;
        
        Randomizer = new Random(zBasedSeed);
        
        // Generate Map
        GenerateElements();
    }

    protected abstract void GenerateElements();
    public override string GetModelPath() => GetModelPath("lanes");
    public override void SetPosition(int zPosition) => Position = Position with { Z = zPosition };

    // lane is 25 * 16 long, one field is 16x16 -> idk field now kinda cursed length? it works tough
    public int GetMapPositionFromLanePositionIndex(int lanePositionIndex) => lanePositionIndex - 12;
    public List<CrossyMapElement> GetElements() => Elements;
    public void AddElement(CrossyMapElement element) => Elements.Add(element);
    public void RemoveElementById(Guid elementId) => Elements.RemoveAll(e => e.Id == elementId);
    public void ClearElements() => Elements.Clear();

    public CrossyMapLaneDto ToDto() => new()
    {
        Id = Id,
        Type = ModelPart,
        Position = Position,
        ModelLocation = GetModelPath(),
        Elements = Elements.Select(e => e.ToDto()).ToList()
    };
}