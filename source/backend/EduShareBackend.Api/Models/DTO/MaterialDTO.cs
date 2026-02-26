using EduShareBackend.Api.Models.Enums;

namespace EduShareBackend.Api.Models.Entities
{
    public class MaterialDTO  //userid subjectid formfajl new prima i vraca od materials vue nov api
    {
        public int MaterialId{get;set;}
        public string Name{get;set;}
        public string DescriptionMaterial{get;set;}
        public MaterialType Type { get; set; }
        public DateTime DateCreated{get;set;}
        public string AuthorName { get; set; }
    
    }
}