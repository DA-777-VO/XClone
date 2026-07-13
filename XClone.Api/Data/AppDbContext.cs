using Microsoft.EntityFrameworkCore;
using XClone.Api.Entities;

namespace XClone.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.TweetId });

        modelBuilder.Entity<Subscription>()
            .HasKey(s => new { s.FollowerId, s.FolloweeId });

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(s => s.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(s => s.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<Tweet> Tweets { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Like> Likes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
}
