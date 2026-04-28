using System.Numerics;

namespace CrossyRoadApi.Models.Map.Elements;

public class Stone1 : CrossyStaticMapElement
{
    public Stone1(CrossyMapLane lane, Vector3 position) : base(CrossyModelPart.STONE_1, lane, position, 1)
    {
    }
}