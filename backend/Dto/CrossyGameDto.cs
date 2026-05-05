using CrossyRoadApi.Models.Game;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Dto;

public class CrossyGameDto
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public List<CrossyPlayerDto> Players { get; set; }
    public int Seed { get; set; }
    public CrossyTheme Theme { get; set; }
    public CrossyGame.Phase GamePhase { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}