using System.Numerics;

namespace CrossyRoadApi.Models.Map;

public abstract class CrossyMapElement : CrossyModel
{
    public CrossyMapLane Lane { get; }
    public int ModelWidth { get; }
    public int Rotation { get; protected set; } = 0;
    
    public CrossyMapElement(CrossyModelPart modelPart, CrossyMapLane lane, Vector3 position, int modelWidth) : base(modelPart, position)
    {
        Lane = lane;
        ModelWidth = modelWidth;
        // Force Z axis to be aligned with the lane
        SetPosition(new Vector3(position.X, position.Y + Lane.BaseYOffset, lane.Position.Z));
        Lane.AddElement(this);
    }
    
    public override string GetModelPath() => GetModelPath("elements");
    
    public void SetRotation(int rotation) => Rotation =  rotation;
}