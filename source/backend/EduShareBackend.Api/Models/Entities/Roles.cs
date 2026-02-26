namespace EduShareBackend.Api.Models.Entities
{
    public class Roles
    {
        public int RoleId{get;set;}
        public string RoleName{get;set;}

       public virtual ICollection<Users> User { get; set; } = new List<Users>();
       public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}