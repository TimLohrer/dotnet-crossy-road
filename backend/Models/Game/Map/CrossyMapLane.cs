using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapLane(CrossyModelPart modelPart, int zPosition, float baseYOffset)
    : CrossyModel(modelPart, new Vector3(0, 0, zPosition))
{
    // The base Y offset applied to all map elements on this lane
    // to make sure they appear on the ground when supplied with the Y coordinate 0
    public float BaseYOffset { get; } = baseYOffset;
    public List<CrossyMapElement> Elements = [];

    public abstract List<CrossyMapElement> GenerateElements();

    public override string GetModelPath() => GetModelPath("lanes");
    public override void SetPosition(int zPosition) => Position = Position with { Z = zPosition };
    
    public List<CrossyMapElement> GetElements() => Elements;
    public void AddElement(CrossyMapElement element) => Elements.Add(element);
    public void RemoveElementById(Guid elementId) => Elements.RemoveAll(e => e.Id == elementId);
    public void ClearElements() => Elements.Clear();
}