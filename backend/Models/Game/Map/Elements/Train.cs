namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Train(CrossyMapLane lane, int xPosition, int xOffset)
    : CrossyMovingMapElement(CrossyModelPart.Train, lane, xPosition, 1, true, 75f, xOffset);