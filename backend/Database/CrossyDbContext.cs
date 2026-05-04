using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Database;

public class CrossyDbContext(DbContextOptions<CrossyDbContext> options)
    : IdentityDbContext<CrossyUser, UserRole, Guid>(options)
{
    public DbSet<CrossyGame> CrossyGames { get; set; }
    public DbSet<CrossyPlayer> CrossyPlayers { get; set; }
}