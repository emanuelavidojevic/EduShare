namespace EduShareBackend.Api.Models.Entities
{
    
    public class UserRatings
    {
        public int RatingsId{get;set;}
        public int UserId{get;set;}
        public int MaterialId{get;set;}
        
        public virtual Users User { get; set; }
        public virtual Ratings Rating { get; set; }
         public virtual Materials Material { get; set; }

    }
}