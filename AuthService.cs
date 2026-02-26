using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiaryApp.Data;
using DiaryApp.DTOs;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DiaryApp.Services;

public interface IAuthService
{
    Task<(AuthResponse? Response, string? Error)> RegisterAsync(RegisterRequest request);
    Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request);
}

public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
    private readonly string _jwtKey = config["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT key not configured");

    public async Task<(AuthResponse?, string?)> RegisterAsync(RegisterRequest req)
    {
        if (await db.Users.AnyAsync(u => u.Email == req.Email))
            return (null, "Пользователь с таким email уже существует.");

        if (await db.Users.AnyAsync(u => u.Username == req.Username))
            return (null, "Имя пользователя уже занято.");

        var user = new User
        {
            Username     = req.Username,
            Email        = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return (BuildToken(user), null);
    }

    public async Task<(AuthResponse?, string?)> LoginAsync(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return (null, "Неверный email или пароль.");

        return (BuildToken(user), null);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private AuthResponse BuildToken(User user)
    {
        var expires = DateTime.UtcNow.AddDays(7);
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,           user.Email),
            new Claim(ClaimTypes.Name,            user.Username)
        };

        var jwt = new JwtSecurityToken(
            claims:           claims,
            expires:          expires,
            signingCredentials: creds);

        return new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(jwt),
            user.Username,
            user.Email,
            expires);
    }
}
