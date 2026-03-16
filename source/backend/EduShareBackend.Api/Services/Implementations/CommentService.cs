using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduShareBackend.Api.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly AplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public CommentService(AplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<object>> GetCommentsByMaterialId(int materialId)
        {
            return await _context.Comments
                .Where(c => c.MaterialId == materialId)
                .Include(c => c.User)
                .OrderByDescending(c => c.DateComments)
                .Select(c => new 
                {
                    c.CommentId,
                    c.TextOfComments,
                    c.DateComments,
                    User = c.User != null ? new 
                    { 
                        FirstName = c.User.FirstName, 
                        LastName = c.User.LastName 
                    } : null
                })
                .ToListAsync();
        }

        public async Task<Comments> AddComment(CommentDTO commentDto, int userId, int materialId)
        {
            var material = await _context.Materials.FindAsync(materialId);
            var user = await _context.Users.FindAsync(userId);
            string authorName = user != null ? $"{user.FirstName} {user.LastName}" : "Neko";

            var comment = new Comments
            {
                TextOfComments = commentDto.TextOfComments,
                DateComments = DateTime.Now,
                UserId = userId,
                MaterialId = materialId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            if (material != null && material.UserId != userId)
            {
                await _notificationService.CreateNotification(
                    title: "Novi komentar",
                    message: $"{authorName} je komentarisao materijal: {material.Name}",
                    type: "Comment",
                    subjectId: material.SubjectId,
                    userId: userId,
                    materialId: materialId
                );
            }

            return comment;
        }
    }
}