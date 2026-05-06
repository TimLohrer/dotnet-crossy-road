namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Car0(CrossyMapLane lane, int xPosition, float speed, int nextCarDelay)
    : CrossyMovingMapElement(CrossyModelPart.Car0, lane, xPosition, 1, true, speed, nextCarDelay);