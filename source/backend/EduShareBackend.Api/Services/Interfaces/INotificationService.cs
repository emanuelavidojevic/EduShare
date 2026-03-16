using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Services.Interfaces
{
public interface INotificationService
{
    Task<NotificationDTO> CreateNotification(string title, string message, string type, int? subjectId = null, int? userId = null, int? materialId = null);
    Task<IEnumerable<NotificationDTO>> GetNotifications();
    Task MarkAsRead();
}
}