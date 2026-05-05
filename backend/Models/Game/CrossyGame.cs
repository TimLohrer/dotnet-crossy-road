using System.ComponentModel.DataAnnotations.Schema;
using CrossyRoadApi.Database;
using CrossyRoadApi.Dto;
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
    public Guid HostId { get; private set; }
    public List<CrossyPlayer> Players { get; set; }
    public int Seed { get; private set; } = new Random().Next();
    public CrossyTheme Theme { get; private set; } = CrossyTheme.Default;
    [NotMapped] public Phase GamePhase { get; set; } = Phase.Created;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public static CrossyGame Create(CrossyPlayer host, CrossyTheme theme, int? seed = null)
    {
        return new CrossyGame
        {
            HostId = host.User.Id,
            Players = [host],
            Theme = theme,
            Seed = seed ?? new Random().Next()
        };
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
        StartTime = DateTime.Now;
        GamePhase = Phase.Active;
    }

    public void EndGame()
    {
        GamePhase = Phase.Ended;
        EndTime = DateTime.Now;
    }

    public async Task SaveGame(CrossyDbContext dbContext)
    {
        if (GamePhase != Phase.Ended) throw new InvalidOperationException("Can only save ended games");
        await dbContext.CrossyGames.AddAsync(this);
        await dbContext.CrossyPlayers.AddRangeAsync(Players);
        await dbContext.SaveChangesAsync();
    }
}