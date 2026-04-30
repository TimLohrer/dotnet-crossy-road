using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;

namespace CrossyRoadApi.Models.Game;

public class CrossyWsPlayer(string connectionId, CrossyPlayer user, Vector3 spawnPosition)
{
    public string Id { get; set; } = connectionId;
    public CrossyPlayerMinimalDto User { get; set; } = user.ToMinimalDto();
    public Vector3 Position { get; set; } = spawnPosition;
    public int FurthestZPosition { get; set; } = 0;
    public int Score { get; set; } = 0;

    public int Taler { get; set; } = 0;
    // public Dictionary<int, char> Keystrokes { get; set; }
}