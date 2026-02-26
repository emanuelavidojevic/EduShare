using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduShareBackend.Api.Data.Configurations
{
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class MaterialDataonfiguration : IEntityTypeConfiguration<MaterialData>
    {
        public void Configure(EntityTypeBuilder<MaterialData> builder)
            {
               builder.HasKey(md => md.MaterialId);
               builder.Property(md => md.MaterialId)
           .ValueGeneratedNever();
            }
        }
    }
}