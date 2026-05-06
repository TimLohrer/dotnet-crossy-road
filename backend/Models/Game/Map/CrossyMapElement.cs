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
    public bool HasCollision { get; } = hasCollision;
    public int ModelWidth { get; } = modelWidth;

    public override string GetModelPath()
    {
        return GetModelPath("elements");
    }

    public override void SetPosition(int xPosition)
    {
        Position = Position with { X = xPosition };
    }

    public abstract CrossyMapElementDto ToDto();
}