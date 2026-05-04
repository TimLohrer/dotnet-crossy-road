using System.ComponentModel.DataAnnotations.Schema;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Game;

public class CrossyWsGame(CrossyWsPlayer host, CrossyTheme theme, int? seed = null)
{
    public enum Phase
    {
        Created,
        Active,
        Ended
    }

    public Guid Id { get; } = Guid.NewGuid();
    public Guid HostId { get; } = host.User.Id;
    public List<CrossyWsPlayer> Players { get; set; } = [host];
    public int Seed { get; } = seed ?? new Random().Next();
    public CrossyTheme Theme { get; } = theme;
    [NotMapped] public Phase GamePhase { get; set; } = Phase.Created;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime? EndTime { get; set; }
}