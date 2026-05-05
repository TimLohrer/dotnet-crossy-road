using CrossyRoadApi.Models.Database;

namespace CrossyRoadApi.Dto;

public class CrossyUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int HighScore { get; set; }
    public int Taler { get; set; }
    public CrossySkin Skin { get; set; }
    public List<CrossySkin> OwnedSkins { get; set; }
}