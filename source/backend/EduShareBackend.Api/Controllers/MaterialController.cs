using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduShareBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTable([FromQuery] MaterialQueryDTO query)
        {
            var materials = await _materialService.TableMaterial(query);
            return Ok(materials);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var result = await _materialService.MaterialDownload(id);
            if (result == null) return NotFound("Fajl nije pronađen.");
            return File(result.Value.Data, result.Value.MimeType, result.Value.FileName);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(300_000_000)]
        public async Task<IActionResult> Upload([FromForm] MaterialUploadDTO dto)
        {
            if (dto.File == null || dto.File.Length == 0) return BadRequest("Fajl je prazan.");
            var success = await _materialService.UploadMaterial(dto);
            return success ? Ok("Uspešno sačuvano.") : BadRequest("Greška pri čuvanju.");
        }

       [HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

    if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

    var userId = int.Parse(userIdClaim);
    var deleted = await _materialService.DeleteMaterialAsync(id, userId, userRole);
    
    return deleted ? Ok("Obrisano.") : Forbid("Nemate dozvolu za brisanje ovog materijala.");
}

        [HttpGet("dashboard-stats/{userId}")]
        public async Task<IActionResult> GetDashboardStats(int userId)
        {
            var stats = await _materialService.GetProfileChartData(userId);
            if (stats == null) return NotFound("Nema podataka za ovog korisnika.");
            return Ok(stats);
        }
    }
}