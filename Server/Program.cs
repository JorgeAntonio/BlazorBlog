using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostDbContext>(options => options.UseSqlite("Data Source=./Data/PostDb.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "web API serving a very simple post model");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors();


app.MapGet("/posts/all", async (PostDbContext dbContext) =>
{
    List<Post> allPost = await dbContext.Posts.ToListAsync();
    return allPost;
});

app.MapGet("/posts/by-id/{postId}", async (int postId, PostDbContext dbContext) =>
{
    Post? post = await dbContext.Posts.FindAsync(postId);
    if (post != null)
    {
        return Results.Ok(post);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/posts/create", async (Post postToCreate, PostDbContext dbContext) =>
{
    dbContext.Posts.Add(postToCreate);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/posts/By-id/{postToCreate.PostId}", postToCreate);
});

app.MapPut("/posts/update/{postId}", async (int postId, Post updatedPost, PostDbContext dbContext) =>
{
    var postToUpdate = await dbContext.Posts.FindAsync(postId);

    if (postToUpdate == null)
    {
        return Results.NotFound();
    }

    postToUpdate.Title = updatedPost.Title;
    postToUpdate.Content = updatedPost.Content;

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/posts/delete/{postId}", async (int postId, PostDbContext dbContext) =>
{
    Post? postToDelete = await dbContext.Posts.FindAsync(postId);

    if (postToDelete != null)
    {
        dbContext.Posts.Remove(postToDelete);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }
});

app.Run();
