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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CrossyPlayer>(entity =>
        {
            entity.HasKey(player => new { player.CrossyGameId, player.UserId });

            entity.HasOne(player => player.CrossyGame)
                .WithMany(game => game.Players)
                .HasForeignKey(player => player.CrossyGameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(player => player.User)
                .WithMany()
                .HasForeignKey(player => player.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<CrossyGame>(entity =>
        {
            entity.HasOne(game => game.Host)
                .WithMany()
                .HasForeignKey(game => game.HostId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });
    }
}