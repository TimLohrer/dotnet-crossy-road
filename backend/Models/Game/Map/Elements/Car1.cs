namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Car1(CrossyMapLane lane, int xPosition, float speed, int nextCarDelay)
    : CrossyMovingMapElement(CrossyModelPart.Car1, lane, xPosition, 2, true, speed, nextCarDelay);