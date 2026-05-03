using System.Numerics;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Dto;

public class CrossyMapElementDto
{
    public Guid Id { get; set; }
    public CrossyModelPart Type { get; set; }
    public Vector3 BasePosition { get; set; }
    public List<Vector3> Positions { get; set; }
    public string ModelLocation { get; set; }
    public bool HasCollision { get; set; }
}