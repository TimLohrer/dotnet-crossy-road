using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Game;

public class CrossyGamePlayer(string connectionId, CrossyPlayer user, Vector3 spawnPosition)
{
    public string Id { get; set; } = connectionId;
    public CrossyPlayerMinimalDto User { get; set; } = user.ToMinimalDto();
    public Vector3 Position { get; set; } = spawnPosition;
    public List<CrossyMapLane> ActiveLanes { get; set; }
    public int Score { get; set; }
    public int Taler { get; set; }
    public Dictionary<int, char> Keystrokes { get; set; }
}