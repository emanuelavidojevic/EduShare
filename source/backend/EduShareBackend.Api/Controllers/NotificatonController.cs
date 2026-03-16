using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetNotifications();
            return Ok(notifications);
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead()
        {
            await _notificationService.MarkAsRead();
            return Ok(new { message = "Sva obaveštenja su označena kao pročitana." });
        }
    }
}