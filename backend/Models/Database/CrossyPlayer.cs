using System.ComponentModel.DataAnnotations;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game;

namespace CrossyRoadApi.Models.Database;

public class CrossyPlayer(string username)
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(35)] public string Username { get; set; } = username;

    public int HighScore { get; set; }
    public int Taler { get; set; }
    public CrossySkin Skin { get; set; } = CrossySkin.Default;
    public List<CrossySkin> OwnedSkins { get; set; } = [CrossySkin.Default];
    public List<CrossyWsGame> Games { get; set; } = [];

    public CrossyPlayerDto ToDto()
    {
        return new CrossyPlayerDto
        {
            Id = Id,
            Username = Username,
            HighScore = HighScore,
            Taler = Taler,
            Skin = Skin,
            OwnedSkins = OwnedSkins
        };
    }

    public CrossyPlayerMinimalDto ToMinimalDto()
    {
        return new CrossyPlayerMinimalDto
        {
            Id = Id,
            Username = Username,
            HighScore = HighScore,
            Skin = Skin
        };
    }
}