namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyStaticMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth,
    bool hasCollision)
    : CrossyMapElement(modelPart, lane, xPosition, modelWidth, hasCollision);