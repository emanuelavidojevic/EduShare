namespace EduShareBackend.Api.Models.Entities
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TypeChange { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }=false;
        public int? SubjectId { get; set; } 
        public int? UserId { get; set; } 
        public int? MaterialId { get; set; }
    }
}