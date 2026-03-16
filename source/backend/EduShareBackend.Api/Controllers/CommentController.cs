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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("material/{materialId}")]
        public async Task<IActionResult> GetComments(int materialId)
        {
            var comments = await _commentService.GetCommentsByMaterialId(materialId);
            return Ok(comments);
        }

        [HttpPost("{materialId}")]
        public async Task<IActionResult> PostComment(int materialId, [FromBody] CommentDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var comment = await _commentService.AddComment(dto, userId, materialId);
            
            return Ok(comment);
        }
    }
}