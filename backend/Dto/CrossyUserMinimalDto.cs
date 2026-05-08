using CrossyRoadApi.Models.Database;

namespace CrossyRoadApi.Dto;

public class CrossyUserMinimalDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int HighScore { get; set; }
    public CrossySkin Skin { get; set; }
}