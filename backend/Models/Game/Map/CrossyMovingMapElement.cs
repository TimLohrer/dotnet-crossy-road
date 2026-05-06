using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMovingMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth,
    bool hasCollision,
    float speed,
    int nextCarDelay)
    : CrossyMapElement(modelPart, lane, xPosition, modelWidth, hasCollision)
{
    public enum ModelDirection
    {
        Right,
        Left
    }
    
    public readonly int NextCarDelay = nextCarDelay;
    public readonly float Speed = speed;
    public readonly ModelDirection Direction = xPosition < 0 ? ModelDirection.Right : ModelDirection.Left;

    public override CrossyMapElementDto ToDto()
    {
        return new CrossyMapElementDto
        {
            Id = Id,
            Type = ModelPart,
            BasePosition = Position,
            ModelWidth = ModelWidth,
            ModelLocation = GetModelPath(),
            HasCollision = HasCollision,
            IsStatic = false,
            Speed = Speed,
            NextCarDelay = NextCarDelay
        };
    }
}