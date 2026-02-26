using DiaryApp.Data;
using DiaryApp.DTOs;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DiaryApp.Services;

public interface IPostService
{
    Task<PostsPageResponse> GetFeedAsync(int userId, int page, int pageSize);
    Task<PostResponse?>     GetByIdAsync(int postId, int userId);
    Task<PostResponse>      CreateAsync(int userId, CreatePostRequest req);
    Task<(PostResponse? Post, string? Error)> UpdateAsync(int postId, int userId, UpdatePostRequest req);
    Task<string?>           DeleteAsync(int postId, int userId);
}

public class PostService(AppDbContext db) : IPostService
{
    // Лента: новые → старые, постраничная
    public async Task<PostsPageResponse> GetFeedAsync(int userId, int page, int pageSize)
    {
        pageSize   = Math.Clamp(pageSize, 1, 100);
        page       = Math.Max(page, 1);

        var query      = db.Posts.Where(p => p.UserId == userId);
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => ToResponse(p))
            .ToListAsync();

        return new PostsPageResponse(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PostResponse?> GetByIdAsync(int postId, int userId)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
        return post is null ? null : ToResponse(post);
    }

    public async Task<PostResponse> CreateAsync(int userId, CreatePostRequest req)
    {
        var post = new Post
        {
            UserId  = userId,
            Title   = req.Title,
            Content = req.Content,
            Mood    = req.Mood
        };
        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return ToResponse(post);
    }

    public async Task<(PostResponse?, string?)> UpdateAsync(int postId, int userId, UpdatePostRequest req)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
        if (post is null) return (null, "Запись не найдена или у вас нет прав на её изменение.");

        post.Title     = req.Title;
        post.Content   = req.Content;
        post.Mood      = req.Mood;
        post.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return (ToResponse(post), null);
    }

    public async Task<string?> DeleteAsync(int postId, int userId)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
        if (post is null) return "Запись не найдена или у вас нет прав на её удаление.";

        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return null;
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static PostResponse ToResponse(Post p) =>
        new(p.Id, p.Title, p.Content, p.Mood, p.CreatedAt, p.UpdatedAt);
}
