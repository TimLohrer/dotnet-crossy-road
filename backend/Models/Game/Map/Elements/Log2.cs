namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Log2(CrossyMapLane lane, int xPosition, float speed, int xOffset)
    : CrossyMovingMapElement(CrossyModelPart.Log2, lane, xPosition, 3, false, speed, xOffset);