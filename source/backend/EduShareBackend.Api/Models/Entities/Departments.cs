namespace EduShareBackend.Api.Models.Entities
{
    public class Departments
    {
        public int DepartmentId{get;set;}
        public string DepartmentName{get;set;}
        public virtual ICollection<Subjects> Subjects { get; set; } = new List<Subjects>();
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();

    }
}