using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;

namespace CrossyRoadApi.Models.Game;

public class CrossyWsPlayer(string connectionId, CrossyPlayer user, Vector3 spawnPosition)
{
    public string ConnectionId { get; } = connectionId;
    public CrossyPlayerMinimalDto User { get; set; } = user.ToMinimalDto();
    public Vector3 Position { get; private set; } = spawnPosition;
    public int FurthestZPosition { get; private set; }
    public int FurthestGeneratedZPosition { get; set; } = 14; // See GenerateMapStart function
    public int Score { get; private set; }

    public int Taler { get; private set; } = 0;
    // public Dictionary<int, char> Keystrokes { get; set; }

    public bool UpdatePosition(Vector3 newPosition)
    {
        var shouldGenerateNewSection = false;
        if (newPosition.Z > Position.Z && newPosition.Z > 0)
        {
            FurthestZPosition = (int)newPosition.Z;
            if (FurthestZPosition + 15 > FurthestGeneratedZPosition)
                shouldGenerateNewSection = true;
            Score++;
        }
        Position = newPosition;
        
        return shouldGenerateNewSection;
    }
}