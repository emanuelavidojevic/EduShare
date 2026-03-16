using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EduShareBackend.Api.Models.Entities;
using System.Security.Claims;

namespace EduShareBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("average/{materialId}")]
        public async Task<IActionResult> GetAverage(int materialId)
        {
            var avg = await _ratingService.GetAverage(materialId);
            return Ok(new { averageRating = avg });
        }

        [HttpPost("{materialId}")]
        public async Task<IActionResult> PostRating(int materialId, [FromBody] RatingDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            int userId = int.Parse(userIdString);
            
            await _ratingService.AddOrUpdate(userId, materialId, dto.ValueForRatings);
            return Ok(new { message = "Ocena uspešno procesuirana." });
        }
    }
}