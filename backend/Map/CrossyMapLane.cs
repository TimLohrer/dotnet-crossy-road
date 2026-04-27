using System.Numerics;

namespace CrossyRoadApi.Map;

public class CrossyMapLane : CrossyModel
{
    // The base Y offset applied to all map elements on this lane
    // to make sure they appear on the ground when supplied with the Y coordinate 0
    public float BaseYOffset { get; }
    
    public CrossyMapLane(CrossyModelPart modelPart, Vector3 position, float baseYOffset, CrossyTheme? theme) : base(modelPart, position,
        theme)
    {
        BaseYOffset = baseYOffset;
    }

    public override string GetModelPath() => GetModelPath("lanes");
}