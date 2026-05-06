using CrossyRoadApi.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Models.Database;

[PrimaryKey(nameof(Id))]
public class CrossyUser : IdentityUser<Guid>
{
    public int HighScore { get; set; }
    public int Taler { get; set; }
    public int Skin { get; set; } = CrossySkin.Chicken.Id;
    public List<int> OwnedSkins { get; set; } = [CrossySkin.Chicken.Id];

    public CrossyUserDto ToDto()
    {
        return new CrossyUserDto
        {
            Id = Id,
            Username = UserName!,
            HighScore = HighScore,
            Taler = Taler,
            Skin = CrossySkin.FromId(Skin),
            OwnedSkins = OwnedSkins.Select(CrossySkin.FromId).ToList()
        };
    }

    public CrossyUserMinimalDto ToMinimalDto()
    {
        return new CrossyUserMinimalDto
        {
            Id = Id,
            Username = UserName!,
            HighScore = HighScore,
            Skin = CrossySkin.FromId(Skin)
        };
    }
}