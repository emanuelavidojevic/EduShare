using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduShareBackend.Api.Models.Entities
{
    public class MaterialData
    {
        [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int MaterialId { get; set; }
        public byte[] Data{get;set;}
        public virtual Materials Material { get; set; }

    
    }
}