using System.Numerics;

namespace CrossyRoadApi.Models.Map.Lanes;

public class PlainsLane : CrossyMapLane
{
    public PlainsLane(Vector3 position) : base(CrossyModelPart.PLAINS, position, PixelSize)
    {
    }

    public override List<CrossyMapElement> GenerateElements() => [];
}