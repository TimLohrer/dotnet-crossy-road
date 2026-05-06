namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Car1(CrossyMapLane lane, int xPosition, float speed, int nextCarDelay)
    : CrossyMovingMapElement(CrossyModelPart.Car1, lane, xPosition, 1, true, speed, nextCarDelay);