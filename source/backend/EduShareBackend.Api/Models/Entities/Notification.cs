using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduShareBackend.Api.Models.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TypeChange { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int? SubjectId { get; set; }
        public int? UserId { get; set; }
        public int? MaterialId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subjects? Subject { get; set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Materials? Material { get; set; }
        public bool IsRead { get; set; } = false;
    }
}