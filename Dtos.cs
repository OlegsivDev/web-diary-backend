using System.ComponentModel.DataAnnotations;

namespace DiaryApp.DTOs;

// ── Auth ──────────────────────────────────────────────────────────────────────

public record RegisterRequest(
    [Required, MinLength(3), MaxLength(50)]  string Username,
    [Required, EmailAddress]                  string Email,
    [Required, MinLength(6)]                  string Password
);

public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Username,
    string Email,
    DateTime ExpiresAt
);

// ── Posts ─────────────────────────────────────────────────────────────────────

public record CreatePostRequest(
    [Required, MaxLength(200)] string Title,
    [Required]                  string Content,
    [MaxLength(50)]             string Mood = ""
);

public record UpdatePostRequest(
    [Required, MaxLength(200)] string Title,
    [Required]                  string Content,
    [MaxLength(50)]             string Mood = ""
);

public record PostResponse(
    int Id,
    string Title,
    string Content,
    string Mood,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PostsPageResponse(
    IEnumerable<PostResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
