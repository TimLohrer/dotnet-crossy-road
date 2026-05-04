using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Database;

public class CrossyDbContext(DbContextOptions<CrossyDbContext> options) : DbContext(options)
{
    public DbSet<CrossyPlayer> CrossyPlayers { get; set; }
    public DbSet<CrossyWsGame> CrossyGames { get; set; }
    public DbSet<CrossyWsPlayer> CrossyGamePlayers { get; set; }
}