namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Lillypad(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Lillypad, lane, xPosition, 1, false)
{
}