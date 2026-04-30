namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMovingMapElement(
    CrossyModelPart modelPart,
    CrossyMapLane lane,
    int xPosition,
    int modelWidth)
    : CrossyMapElement(modelPart, lane, xPosition, modelWidth);