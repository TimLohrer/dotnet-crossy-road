using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth,
    bool hasCollision)
    : CrossyModel(modelPart, lane.Position with { X = xPosition })
{
    public enum ModelDirection
    {
        Right,
        Left
    }

    public bool HasCollision { get; } = hasCollision;
    public int ModelWidth { get; } = modelWidth;
    public ModelDirection Direction { get; protected set; } = ModelDirection.Left;

    public override string GetModelPath()
    {
        return GetModelPath("elements");
    }

    public override void SetPosition(int xPosition)
    {
        Position = Position with { X = xPosition };
    }

    public void SetDirection(ModelDirection direction)
    {
        Direction = direction;
    }

    public CrossyMapElementDto ToDto()
    {
        return new CrossyMapElementDto
        {
            Id = Id,
            Type = ModelPart,
            BasePosition = Position,
            ModelWidth = ModelWidth,
            ModelLocation = GetModelPath(),
            HasCollision = HasCollision
        };
    }
}