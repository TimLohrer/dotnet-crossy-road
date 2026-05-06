using System.Numerics;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Dto;

public class CrossyMapElementDto
{
    public Guid Id { get; set; }
    public CrossyModelPart Type { get; set; }
    public Vector3 BasePosition { get; set; }
    public int ModelWidth { get; set; }
    public string ModelLocation { get; set; }
    public bool HasCollision { get; set; }
    public bool IsStatic { get; set; }
    public float Speed { get; set; }
    public int NextCarDelay { get; set; }
    public CrossyMovingMapElement.ModelDirection Direction { get; set; }
}