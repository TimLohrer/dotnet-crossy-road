using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Dto;

public class CrossyGameDto
{
    public Guid Id { get; set; }
    public int Seed { get; set; }
    public CrossyPlayerDto Host { get; set; }
    public List<CrossyPlayerDto> Players { get; set; }
    public CrossyTheme Theme { get; set; }
}