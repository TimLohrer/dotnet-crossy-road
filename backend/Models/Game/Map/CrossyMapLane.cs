using System.Numerics;
using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapLane(CrossyModelPart modelPart, int zPosition, float baseYOffset)
    : CrossyModel(modelPart, new Vector3(0, 0, zPosition))
{
    // The base Y offset applied to all map elements on this lane
    // to make sure they appear on the ground when supplied with the Y coordinate 0
    public float BaseYOffset { get; } = baseYOffset;
    public List<CrossyMapElement> Elements = [];

    public abstract void GenerateElements(int seed);
    public override string GetModelPath() => GetModelPath("lanes");
    public override void SetPosition(int zPosition) => Position = Position with { Z = zPosition };

    // lane is 24 * 16 long, one field is 16x16
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