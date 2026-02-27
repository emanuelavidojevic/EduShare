/*using Microsoft.EntityFrameworkCore;
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

       public async Task<NotificationDTO> CreateNotification(
    string title, 
    string message, 
    string type, 
    int? subjectId = null, 
    int? userId = null, 
    int? materialId = null) 
{
    var notification = new Notification
    {
        Title = title,
        Message = message,
        TypeChange = type,
        Timestamp = DateTime.Now,
        SubjectId = subjectId,
        UserId = userId,
        MaterialId = materialId // Mapiranje
    };

    _context.Notifications.Add(notification);
    await _context.SaveChangesAsync();

    return new NotificationDTO {
        // ... ostala polja ...
        MaterialId = notification.MaterialId
    };
}
    }
}*/