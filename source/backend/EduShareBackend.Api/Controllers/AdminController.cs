using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Uklonjen [Authorize] da bi frontend mogao nesmetano da komunicira
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) => _adminService = adminService;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search = null, [FromQuery] int? roleId = null)
        {
            var users = await _adminService.GetAllUsersAsync(search, roleId);
            return Ok(users);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminDTO dto)
        {
            var result = await _adminService.UpdateUserAsync(id, dto);
            if (!result) return NotFound(new { message = "Korisnik nije pronadjen" });
            return Ok(new { message = "Korisnik uspesno azuriran" });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result) return NotFound(new { message = "Korisnik nije pronadjen" });
            return Ok(new { message = "Korisnik uspesno obrisan" });
        }
    }
}