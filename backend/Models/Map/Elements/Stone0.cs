using System.Numerics;

namespace CrossyRoadApi.Models.Map.Elements;

public class Stone0 : CrossyStaticMapElement
{
    public Stone0(CrossyMapLane lane, Vector3 position) : base(CrossyModelPart.STONE_0, lane, position, 1)
    {
    }
}