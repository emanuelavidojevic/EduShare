using EduShareBackend.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduShareBackend.Api.Models.Entities
{
   using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Materials
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaterialId { get; set; }

    public string Name { get; set; }
    public string DescriptionMaterial { get; set; }
    public virtual Users User { get; set; }
    public int UserId { get; set; }
    public virtual Subjects Subject { get; set; }
    public int SubjectId { get; set; }
    public virtual ICollection<Comments> Comment { get; set; }
    public virtual ICollection<Ratings> Rating { get; set; }

    public DateTime DateCreated { get; set; }
    public int Type { get; set; }

    // Veze
    public virtual MaterialData MaterialData { get; set; }
}
}