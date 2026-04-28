using System.ComponentModel.DataAnnotations.Schema;
using CrossyRoadApi.Models.Game;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Database;

public class CrossyGame
{
    public Guid Id { get; set; }
    [NotMapped] public List<CrossyMapLane> Map { get; set; } = new();

}