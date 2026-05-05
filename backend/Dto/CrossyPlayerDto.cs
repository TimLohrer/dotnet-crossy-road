using System.Numerics;

namespace CrossyRoadApi.Dto;

public class CrossyPlayerDto
{
    public string ConnectionId { get; set; }
    public CrossyUserMinimalDto User { get; set; }
    public Vector3 Position { get; set; }
    public int Score { get; set; }
    public int Taler { get; set; }
    public DateTime? DiedAt { get; set; }
    public bool IsAlive { get; set; }
}