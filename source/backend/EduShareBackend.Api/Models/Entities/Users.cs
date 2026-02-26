namespace EduShareBackend.Api.Models.Entities
{
    public class Users
    {
        public int  UserId{get; set;}
        public string FirstName{get; set;}
        public string LastName{get; set;}
        public string Email{get; set;}
        public string Password{get; set;}
        public DateTime DateRegistration{get; set;}
        public string RefreshToken { get; set; } = string.Empty;
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public virtual ICollection<Materials> Materials { get; set; } = new List<Materials>();
        public int? DepartmentId { get; set; } 
        public virtual Departments Department { get; set; }
        public virtual ICollection<Roles> Roles { get; set; } = new List<Roles>();
        public virtual ICollection<Comments> Comment { get; set; } = new List<Comments>();
         public virtual ICollection<Ratings> Rating { get; set; } = new List<Ratings>();
         public virtual ICollection<UserRoles> UserRoles { get; set; }=new List<UserRoles>();
   
    }
}