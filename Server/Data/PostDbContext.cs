using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public sealed class PostDbContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();

    public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var postToSeed = new Post[6];

        for (int i = 1; i <= 6; i++)
        {
            postToSeed[i - 1] = new Post()
            {
                PostId = i,
                Title = $"Post {i}",
                Content = $"This is a post made in a .net minimal web api"
            };
        }

        builder.Entity<Post>().HasData(postToSeed);
    }
}
