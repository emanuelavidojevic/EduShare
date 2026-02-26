using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduShareBackend.Api.Data.Configurations
{
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class  MaterialConfiguration : IEntityTypeConfiguration<Materials>
    {

    public void Configure(EntityTypeBuilder<Materials> builder)
{
    builder.HasKey(m => m.MaterialId);
    
    builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
    builder.Property(m => m.DescriptionMaterial).HasMaxLength(500);

    builder.HasOne(s => s.Subject)
           .WithMany(m => m.Materials)
           .HasForeignKey(m => m.SubjectId);

    // Veza sa podacima
    builder.HasOne(m => m.MaterialData)
           .WithOne(md => md.Material)
           .HasForeignKey<MaterialData>(md => md.MaterialId)
           .OnDelete(DeleteBehavior.Cascade);
}
                
            }
    }
}
