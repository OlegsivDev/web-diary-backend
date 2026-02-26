using DiaryApp.DTOs;
using DiaryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiaryApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    /// <summary>Регистрация нового пользователя</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (response, error) = await auth.RegisterAsync(req);
        return error is null ? Ok(response) : BadRequest(new { error });
    }

    /// <summary>Вход в систему</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (response, error) = await auth.LoginAsync(req);
        return error is null ? Ok(response) : Unauthorized(new { error });
    }
}
