using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public abstract class CrossyMapLane : CrossyModel
{
    // The base Y offset applied to all map elements on this lane
    // to make sure they appear on the ground when supplied with the Y coordinate 0
    public float BaseYOffset { get; }
    public List<CrossyMapElement> Elements = new();
    
    public CrossyMapLane(CrossyModelPart modelPart, Vector3 position, float baseYOffset) : base(modelPart, position)
    {
        BaseYOffset = baseYOffset;
        Position = position with { X = 0 };
    }
    
    public abstract List<CrossyMapElement> GenerateElements();

    public override string GetModelPath() => GetModelPath("lanes");
    
    public List<CrossyMapElement> GetElements() => Elements;
    public void AddElement(CrossyMapElement element) => Elements.Add(element);
    public void RemoveElementById(Guid elementId) => Elements.RemoveAll(e => e.Id == elementId);
    public void ClearElements() => Elements.Clear();
}