using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Stone1(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Stone_1, lane, xPosition, 1);