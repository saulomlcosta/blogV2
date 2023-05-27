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

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new PostMap());
        modelBuilder.ApplyConfiguration(new UserMap());
    }
}