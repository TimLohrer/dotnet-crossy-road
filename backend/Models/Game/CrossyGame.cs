using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using CrossyRoadApi.Database;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game.Map;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Models.Game;

[PrimaryKey(nameof(Id))]
public class CrossyGame
{
    public enum Phase
    {
        Created,
        Active,
        Ended
    }

    public Guid Id { get; } = Guid.NewGuid();
    public Guid? HostId { get; private set; }
    public CrossyUser? Host { get; set; }
    public List<CrossyPlayer> Players { get; set; } = [];
    public int Seed { get; private set; } = new Random().Next();
    public CrossyTheme Theme { get; private set; } = CrossyTheme.Default;
    [NotMapped] public Phase GamePhase { get; set; } = Phase.Created;
    [NotMapped] public List<Vector3> TalerLocations { get; set; } = [];
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public static CrossyGame Create(CrossyPlayer host, CrossyTheme theme, int? seed = null)
    {
        var game = new CrossyGame
        {
            HostId = host.UserId,
            Players = [host],
            Theme = theme,
            Seed = seed ?? new Random().Next()
        };

        host.CrossyGameId = game.Id;
        return game;
    }

    public CrossyGameDto ToDto()
    {
        return new CrossyGameDto
        {
            Id = Id,
            HostId = HostId,
            Theme = Theme,
            Seed = Seed,
            Players = Players.Select(p => p.ToDto()).ToList(),
            GamePhase = GamePhase,
            StartTime = StartTime,
            EndTime = EndTime
        };
    }

    public void StartGame()
    {
        StartTime = DateTime.UtcNow;
        GamePhase = Phase.Active;
    }

    public void EndGame()
    {
        GamePhase = Phase.Ended;
        EndTime = DateTime.UtcNow;
    }

    public async Task SaveGame(CrossyDbContext dbContext)
    {
        if (GamePhase != Phase.Ended) throw new InvalidOperationException("Can only save ended games");

        foreach (var player in Players)
        {
            player.CrossyGameId = Id;
            player.UserId = player.User?.Id ?? player.UserId;
            player.User = null;
        }

        await dbContext.CrossyPlayers.AddRangeAsync(Players);
        await dbContext.CrossyGames.AddAsync(this);
        await dbContext.SaveChangesAsync();
    }
}