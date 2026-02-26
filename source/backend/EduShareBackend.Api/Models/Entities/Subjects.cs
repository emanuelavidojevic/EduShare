using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduShareBackend.Api.Models.Entities
{
    public class Subjects
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectId{get;set;}
        public string NameSubject{get;set;}
        public string DescriptionSubject{get;set;}
        public virtual Departments Department { get; set; }
        public int DepartmentId { get; set; }


        public virtual ICollection<Materials> Materials { get; set; } = new List<Materials>();

    }
}