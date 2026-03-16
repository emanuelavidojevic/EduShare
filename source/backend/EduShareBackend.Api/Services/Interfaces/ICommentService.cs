using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<object>> GetCommentsByMaterialId(int materialId);
        Task<Comments> AddComment(CommentDTO commentDto, int userId, int materialId);
    }
}