using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using CrossyRoadApi.Dto;

namespace CrossyRoadApi.Models.Database;

public class CrossyPlayer(string username)
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(35)] public string Username { get; set; } = username;

    public int HighScore { get; set; }
    public int Taler { get; set; }
    public CrossySkin Skin { get; set; } = CrossySkin.Default;
    public List<CrossySkin> OwnedSkins { get; set; } = [CrossySkin.Default];

    [NotMapped] public Vector3 Position { get; set; } = new(0, 0, -2);

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
}