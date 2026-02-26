using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Models.Enums;
using Microsoft.AspNetCore.Http;


namespace EduShareBackend.Api.Models
{
    public class MaterialUploadDTO
    {
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
        public MaterialType? Type { get; set; }
    }
}