namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Log1(CrossyMapLane lane, int xPosition, float speed, int xOffset)
    : CrossyMovingMapElement(CrossyModelPart.Log1, lane, xPosition, 2, false, speed, xOffset);