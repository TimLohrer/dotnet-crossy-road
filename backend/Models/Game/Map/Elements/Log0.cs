namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Log0(CrossyMapLane lane, int xPosition, float speed, int xOffset)
    : CrossyMovingMapElement(CrossyModelPart.Log0, lane, xPosition, 1, false, speed, xOffset);