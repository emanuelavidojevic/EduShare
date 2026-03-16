using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;

namespace EduShareBackend.Api.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly AplicationDbContext _context;

        public NotificationService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDTO> CreateNotification(string title, string message, string type, int? subjectId = null, int? userId = null, int? materialId = null) 
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                TypeChange = type,
                Timestamp = DateTime.Now,
                IsRead = false,
                SubjectId = subjectId,
                UserId = userId,
                MaterialId = materialId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return new NotificationDTO 
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                TypeChange = notification.TypeChange,
                Timestamp = notification.Timestamp,
                IsRead = notification.IsRead,
                SubjectId = notification.SubjectId,
                UserId = notification.UserId,
                MaterialId = notification.MaterialId
            };
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotifications()
        {
            return await _context.Notifications
                .Include(n => n.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .OrderByDescending(n => n.Timestamp)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    TypeChange = n.TypeChange,
                    Timestamp = n.Timestamp,
                    IsRead = n.IsRead,
                    SubjectId = n.SubjectId,
                    UserId = n.UserId,
                    MaterialId = n.MaterialId,
                    AuthorName = n.User != null 
                        ? (n.User.UserRoles.Any(ur => ur.Role.RoleName == "Professor") 
                            ? "prof. " + n.User.FirstName + " " + n.User.LastName 
                            : n.User.FirstName + " " + n.User.LastName)
                        : "Sistem"
                })
                .ToListAsync();
        }

        public async Task MarkAsRead()
        {
            var unread = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}