namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Tree2(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Tree2, lane, xPosition, 1, true);