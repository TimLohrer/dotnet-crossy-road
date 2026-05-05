using CrossyRoadApi.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Models.Database;

[PrimaryKey(nameof(Id))]
public class CrossyUser : IdentityUser<Guid>
{
    public int HighScore { get; set; }
    public int Taler { get; set; }
    public CrossySkin Skin { get; set; } = CrossySkin.Default;
    public List<CrossySkin> OwnedSkins { get; set; } = [CrossySkin.Default];

    public CrossyPlayerDto ToDto()
    {
        return new CrossyPlayerDto
        {
            Id = Id,
            Username = UserName,
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
            Username = UserName,
            HighScore = HighScore,
            Skin = Skin
        };
    }
}