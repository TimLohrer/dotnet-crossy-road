using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Game;

public class CrossyWsPlayer(string connectionId, CrossyPlayer user, Vector3 spawnPosition)
{
    [NotMapped] public string ConnectionId { get; } = connectionId;
    [NotMapped] public CrossyPlayerMinimalDto User { get; } = user.ToMinimalDto();
    public Guid Id { get; } = user.Id;
    public Vector3 Position { get; private set; } = spawnPosition;
    [NotMapped] public int FurthestZPosition { get; private set; }
    [NotMapped] public int FurthestGeneratedZPosition { get; set; } = 14; // See GenerateMapStart function
    [NotMapped] public CrossyModelPart LastGeneratedSectionType { get; set; }
    public int Score { get; private set; }
    public int Taler { get; private set; } = 0;
    public DateTime? DiedAt { get; set; }

    [NotMapped] public bool IsAlive => DiedAt == null;
    // public Dictionary<int, char> Keystrokes { get; set; }

    public bool UpdatePosition(Vector3 newPosition)
    {
        var shouldGenerateNewSection = false;
        if (newPosition.Z > FurthestZPosition && newPosition.Z > 0)
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