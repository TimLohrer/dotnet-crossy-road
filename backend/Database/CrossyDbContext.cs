using CrossyRoadApi.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Database;

public class CrossyDbContext : DbContext
{
    public CrossyDbContext(DbContextOptions<CrossyDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<CrossyGame> CrossyGames { get; set; }
}