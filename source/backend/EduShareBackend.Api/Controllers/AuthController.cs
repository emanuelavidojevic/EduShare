using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduShareBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            try {
                await _authService.RegisterAsync(dto);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            try {
                return Ok(await _authService.LoginAsync(dto));
            } catch (UnauthorizedAccessException ex) {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> Refresh()
        {
            var rf = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(rf)) return Unauthorized();
            try {
                return Ok(await _authService.RefreshAccessTokenAsync(rf));
            } catch {
                return Unauthorized();
            }
        }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null) return Unauthorized();
        return Ok(user);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        
        return Ok(new { message = "Odjava uspešna" });
    }
}
}
