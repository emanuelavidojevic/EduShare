namespace EduShareBackend.Api.Models.Entities
{
    public class Ratings
    {
        public int RatingId{get;set;}
        public int ValueForRatings {get;set;}

       public int UserId { get; set; }
        public virtual Users User { get; set; }

        public int MaterialId { get; set; }
        public virtual Materials Material { get; set; }



    }
}