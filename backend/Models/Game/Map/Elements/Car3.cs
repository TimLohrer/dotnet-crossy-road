namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Car3(CrossyMapLane lane, int xPosition, float speed, int nextCarDelay)
    : CrossyMovingMapElement(CrossyModelPart.Car3, lane, xPosition, 1, true, speed, nextCarDelay);