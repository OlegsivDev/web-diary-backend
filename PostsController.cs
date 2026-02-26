using System.Security.Claims;
using DiaryApp.DTOs;
using DiaryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiaryApp.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController(IPostService posts) : ControllerBase
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── GET /api/posts?page=1&pageSize=10 ─────────────────────────────────────
    /// <summary>Лента постов (новые → старые), постраничная</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PostsPageResponse), 200)]
    public async Task<IActionResult> GetFeed(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await posts.GetFeedAsync(CurrentUserId, page, pageSize);
        return Ok(result);
    }

    // ── GET /api/posts/{id} ───────────────────────────────────────────────────
    /// <summary>Получить конкретную запись по ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await posts.GetByIdAsync(id, CurrentUserId);
        return post is null ? NotFound(new { error = "Запись не найдена." }) : Ok(post);
    }

    // ── POST /api/posts ───────────────────────────────────────────────────────
    /// <summary>Создать новую запись в дневнике</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PostResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var post = await posts.CreateAsync(CurrentUserId, req);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    // ── PUT /api/posts/{id} ───────────────────────────────────────────────────
    /// <summary>Обновить запись полностью</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (post, error) = await posts.UpdateAsync(id, CurrentUserId, req);
        return error is null ? Ok(post) : NotFound(new { error });
    }

    // ── DELETE /api/posts/{id} ────────────────────────────────────────────────
    /// <summary>Удалить запись</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var error = await posts.DeleteAsync(id, CurrentUserId);
        return error is null ? NoContent() : NotFound(new { error });
    }
}
