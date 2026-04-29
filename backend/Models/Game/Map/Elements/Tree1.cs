namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Tree1(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Tree1, lane, xPosition, 1);