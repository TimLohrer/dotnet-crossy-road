using System.ComponentModel.DataAnnotations.Schema;
using CrossyRoadApi.Models.Map;

namespace CrossyRoadApi.Models.Database;

public class CrossyGame
{
    public Guid Id { get; set; }
    [NotMapped]
    public List<CrossyMapLane> Map { get; set; } = new()
    
}