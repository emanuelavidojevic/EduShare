namespace EduShareBackend.Api.Models.Entities
{
    public class Comments
    {
        public int CommentId{get;set;}
        public string TextOfComments{get;set;}
        public DateTime DateComments{get;set;}

        public int UserId { get; set; }
        public virtual Users User { get; set; }

        public int MaterialId { get; set; }
        public virtual Materials Material { get; set; }

    }
}