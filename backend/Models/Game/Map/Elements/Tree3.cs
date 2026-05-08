namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Tree3(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Tree3, lane, xPosition, 1, true);