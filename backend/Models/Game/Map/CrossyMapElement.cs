using System.Numerics;
using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapElement(CrossyModelPart modelPart, CrossyMapLane lane, int xPosition, int modelWidth)
    : CrossyModel(modelPart, lane.Position with { X = xPosition })
{
    public int ModelWidth { get; } = modelWidth;
    public CrossyDirection Direction { get; protected set; } = CrossyDirection.Left;

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