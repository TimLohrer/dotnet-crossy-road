using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Utils;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapLane : CrossyModel
{
    protected static int LaneLength = 25;
    public List<CrossyMapElement> Elements = [];

    public CrossyMapLane(CrossyModelPart modelPart, int seed, int zPosition, bool empty = false) : base(modelPart,
        new Vector3(0, 0, zPosition))
    {
        // Generate Map
        if (!empty)
        {
            Randomizer = CrossyRandomizer.Get(seed, zPosition);
            GenerateElements();
        }
    }

    protected Random Randomizer { get; }
    public abstract CrossyModelPart LaneType { get; }

    protected abstract void GenerateElements();

    public override string GetModelPath()
    {
        return GetModelPath("lanes");
    }

    public override void SetPosition(int zPosition)
    {
        Position = Position with { Z = zPosition };
    }

    // lane is 25 * 16 long, one field is 16x16 -> idk field now kinda cursed length? it works tough
    public int GetMapPositionFromLanePositionIndex(int lanePositionIndex)
    {
        return lanePositionIndex - 12;
    }

    public List<CrossyMapElement> GetElements()
    {
        return Elements;
    }

    public void AddElement(CrossyMapElement element)
    {
        Elements.Add(element);
    }

    public void RemoveElementById(Guid elementId)
    {
        Elements.RemoveAll(e => e.Id == elementId);
    }

    public void ClearElements()
    {
        Elements.Clear();
    }

    public CrossyMapLaneDto ToDto()
    {
        return new CrossyMapLaneDto
        {
            Id = Id,
            Type = ModelPart,
            Position = Position,
            ModelLocation = GetModelPath(),
            Elements = Elements.Select(e => e.ToDto()).ToList()
        };
    }
}