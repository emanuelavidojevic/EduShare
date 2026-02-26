using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetTable()
        {
            var materials = await _materialService.TableMaterial();
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
            var deleted = await _materialService.DeleteMaterialAsync(id);
            return deleted ? Ok("Obrisano.") : NotFound();
        }
        [HttpGet("user-stats/{userId}")]
    public async Task<IActionResult> GetStats(int userId)
    {
        var stats = await _materialService.GetUserMaterialStats(userId);
        return Ok(stats);
    }

    [HttpGet("user-images/{userId}")]
    public async Task<IActionResult> GetImages(int userId)
    {
        var images = await _materialService.GetUserImagesAsync(userId);
        return Ok(images);
    }
    }
}