namespace EduShareBackend.Api.Models.Entities
{
    
    public class UserComments
    {
        public int CommentId{get;set;}
        public int UserId{get;set;}
          public int MaterialId { get; set; }
        
        public virtual Users User { get; set; }
        public virtual Comments Comment { get; set; }
          public virtual Materials Material { get; set; }

    }
}