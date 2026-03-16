using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Models
{
    public class SubjectDTO{
        public int SubjectId{get;set;}
        public string NameSubject{get;set;}
        public string DescriptionSubject{get;set;}
        public int MaterialCount { get; set; }
        public int DepartmentId { get; set; }
        public string ProfessorName { get; set; }
        public int UserId{get;set;}
    }
}
