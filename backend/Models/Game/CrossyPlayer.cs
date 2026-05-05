using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Game;

public class CrossyPlayer
{
    [NotMapped] public string ConnectionId { get; private set; } = null!;

    [Required] public Guid CrossyGameId { get; set; }
    public virtual CrossyGame? CrossyGame { get; set; }

    [Required] [ForeignKey(nameof(User))] public Guid UserId { get; set; }

    public virtual CrossyUser? User { get; set; }

    [NotMapped]
    public Vector3 Position
    {
        get => new(X, Y, Z);
        set
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    [NotMapped] public int FurthestZPosition { get; private set; }
    [NotMapped] public int FurthestGeneratedZPosition { get; set; } = 14; // See GenerateMapStart function
    [NotMapped] public CrossyModelPart LastGeneratedSectionType { get; set; }
    public int Score { get; private set; }
    public int Taler { get; private set; }
    public DateTime? DiedAt { get; set; }

    [NotMapped] public bool IsAlive => DiedAt == null;
    // public Dictionary<int, char> Keystrokes { get; set; }

    public static CrossyPlayer Create(string connectionId, CrossyUser user, Vector3 spawnPosition)
    {
        return new CrossyPlayer
        {
            ConnectionId = connectionId,
            UserId = user.Id,
            User = user,
            Position = spawnPosition
        };
    }

    public CrossyPlayerDto ToDto()
    {
        return new CrossyPlayerDto
        {
            ConnectionId = ConnectionId,
            User = (User ?? throw new InvalidOperationException("Player user is not loaded")).ToMinimalDto(),
            Position = Position,
            Taler = Taler,
            Score = Score,
            DiedAt = DiedAt,
            IsAlive = IsAlive
        };
    }

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