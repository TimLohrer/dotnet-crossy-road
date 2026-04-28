using System.Numerics;

namespace CrossyRoadApi.Models.Game.Map.Elements;

public class Stone0(CrossyMapLane lane, int xPosition)
    : CrossyStaticMapElement(CrossyModelPart.Stone_0, lane, xPosition, 1);