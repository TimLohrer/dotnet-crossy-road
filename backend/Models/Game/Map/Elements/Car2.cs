namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Car2(CrossyMapLane lane, int xPosition, float speed, int nextCarDelay)
    : CrossyMovingMapElement(CrossyModelPart.Car2, lane, xPosition, 2, true, speed, nextCarDelay);