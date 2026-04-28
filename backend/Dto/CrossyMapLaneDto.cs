using System.Numerics;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Dto;

public class CrossyMapLaneDto
{
    public Guid Id { get; set; }
    public CrossyModelPart Type { get; set; }
    public Vector3 Position { get; set; }
    public string ModelLocation { get; set; }
    public List<CrossyMapElementDto> Elements { get; set; }
}