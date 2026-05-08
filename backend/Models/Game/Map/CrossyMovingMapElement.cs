using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMovingMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth,
    bool hasCollision,
    float speed,
    int xOffest)
    : CrossyMapElement(modelPart, lane, xPosition, modelWidth, hasCollision)
{
    public enum ModelDirection
    {
        Right,
        Left
    }

    public readonly ModelDirection Direction = xPosition < 0 ? ModelDirection.Left : ModelDirection.Right;
    public readonly float Speed = speed;

    public readonly int XOffset = xOffest;

    public override CrossyMapElementDto ToDto()
    {
        return new CrossyMapElementDto
        {
            Id = Id,
            Type = ModelPart,
            BasePosition = Position,
            Direction = Direction,
            ModelWidth = ModelWidth,
            ModelLocation = GetModelPath(),
            HasCollision = HasCollision,
            IsStatic = false,
            Speed = Speed,
            XOffset = XOffset
        };
    }
}