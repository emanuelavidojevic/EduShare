using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EduShareBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri preuzimanju korisnika.", details = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(UserDTO UserDTO)
        {
            try
            {
                var createdUser = await _userService.CreateAsync(UserDTO);
                return CreatedAtAction(nameof(GetAll), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri kreiranju korisnika.", details = ex.Message });
            }
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole(int UserId, int RoleId)
        {
            try
            {
                var result = await _userService.AddRole(UserId, RoleId);
                if (result.Contains("Error") || result.Contains("Nema"))
                {
                    return BadRequest(new { poruka = result });
                }
                return Ok(new { poruka = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { poruka = "Doslo je do greske!", detalji = ex.Message });
            }
        }
    }
}