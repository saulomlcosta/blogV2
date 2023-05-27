using BlogV2.Models;
using BlogV2.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BlogV2.Data;

public class BlogDataContext : DbContext
{
    public BlogDataContext(DbContextOptions<BlogDataContext> options)
        : base(options)
    {
    }

    public DbSet<Category>? Categories { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new PostMap());
        modelBuilder.ApplyConfiguration(new UserMap());
    }
}