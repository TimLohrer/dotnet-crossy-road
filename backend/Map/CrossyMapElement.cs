using System.Numerics;

namespace CrossyRoadApi.Map;

public abstract class CrossyMapElement : CrossyModel
{
    public CrossyMapLane Lane { get; }
    
    public CrossyMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, CrossyTheme? theme) : base(modelPart, position,
        theme)
    {
        Lane = lane;
        // Force Z axis to be aligned with the lane
        SetPosition(new Vector3(position.X, position.Y + Lane.BaseYOffset, lane.Position.Z));
    }
    
    public override string GetModelPath() => GetModelPath("elements");
}