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
    }
    
    public DbSet<Tweet> Tweets { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<Like> Likes { get; set; }
}