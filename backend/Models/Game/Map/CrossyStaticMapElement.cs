using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyStaticMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth,
    bool hasCollision)
    : CrossyMapElement(modelPart, lane, xPosition, modelWidth, hasCollision)
{
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
            IsStatic = true
        };
    }
}