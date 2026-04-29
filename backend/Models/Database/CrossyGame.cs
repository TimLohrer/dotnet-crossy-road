using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Database;

public class CrossyGame(CrossyPlayer host, CrossyTheme theme, int? seed = null)
{
    public Guid Id { get; set; }
    public int Seed { get; set; } = seed ?? new Random().Next();
    public CrossyPlayer Host { get; set; } = host;
    public List<CrossyPlayer> Players { get; set; } = [host];
    public int Score { get; set; }
    public int Taler { get; set; }
    public CrossyTheme Theme { get; set; } = theme;

    public CrossyGameDto ToDto()
    {
        return new CrossyGameDto
        {
            Id = Id,
            Seed = Seed,
            Host = Host.ToDto(),
            Players = Players.Select(p => p.ToDto()).ToList(),
            Theme = Theme
        };
    }
}