using System.ComponentModel.DataAnnotations.Schema;
using CrossyRoadApi.Models.Game.Map;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Models.Game;

[PrimaryKey(nameof(Id))]
public class CrossyGame
{
    public enum Phase
    {
        Created,
        Active,
        Ended
    }

    public Guid Id { get; } = Guid.NewGuid();
    public Guid HostId { get; private set; }
    public List<CrossyPlayer> Players { get; set; }
    public int Seed { get; private set; } = new Random().Next();
    public CrossyTheme Theme { get; private set; } = CrossyTheme.Default;
    [NotMapped] public Phase GamePhase { get; set; } = Phase.Created;
    public DateTime StartTime { get; } = DateTime.Now;
    public DateTime? EndTime { get; set; }

    public static CrossyGame Create(CrossyPlayer host, CrossyTheme theme, int? seed = null)
    {
        return new CrossyGame
        {
            HostId = host.User.Id,
            Theme = theme,
            Seed = seed ?? new Random().Next()
        };
    }
}