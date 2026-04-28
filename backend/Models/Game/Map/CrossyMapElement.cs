using System.Numerics;
using System.Reflection;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game.Map.Elements;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapElement : CrossyModel
{
    public CrossyMapLane Lane { get; }
    public int ModelWidth { get; }
    public CrossyDirection Direction { get; protected set; } = CrossyDirection.Left;
    
    public CrossyMapElement(CrossyModelPart modelPart, CrossyMapLane lane, int xPosition, int modelWidth) : base(modelPart, new Vector3(xPosition, lane.BaseYOffset, lane.Position.Z))
    {
        Lane = lane;
        ModelWidth = modelWidth;
        Lane.AddElement(this);
    }
    
    public override string GetModelPath() => GetModelPath("elements");
    public override void SetPosition(int xPosition) => Position = Position with { X = xPosition };

    public void SetDirection(CrossyDirection direction) => Direction = direction;
    public List<Vector3> GetPositions()
    {
        var positions = new List<Vector3>();
        for (var x = 0; x < ModelWidth; x++)
        {
            switch (Direction)
            {
                case CrossyDirection.Left:
                    positions.Add(Position - new Vector3(x, 0, 0));
                    break;
                case CrossyDirection.Right:
                    positions.Add(Position + new Vector3(x, 0, 0));
                    break;
            }
        }
        return positions;
    }

    public CrossyMapElementDto ToDto() => new()
    {
        Id = Id,
        Type = ModelPart,
        BasePosition = Position,
        Positions = GetPositions(),
        ModelLocation = GetModelPath(),
    };
}