using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Game;

public class CrossyGameGame(CrossyGamePlayer host, CrossyTheme theme, int? seed = null)
{
    public Guid Id { get; } = Guid.NewGuid();
    public CrossyGamePlayer Host { get; } = host;
    public List<CrossyGamePlayer> Players { get; set; }
    public int Seed { get; } = seed ?? new Random().Next();
    public CrossyTheme Theme { get; } = theme;
}