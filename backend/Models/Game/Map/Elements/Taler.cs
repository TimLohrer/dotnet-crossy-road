namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Taler(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Taler, lane, xPosition, 1, false);