using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class  DepartmentsConfiguration : IEntityTypeConfiguration<Departments>
    {
        public void Configure(EntityTypeBuilder<Departments> builder)
        {
            builder.HasKey(d => d.DepartmentId);
              builder.Property(d=>d.DepartmentId)
                .ValueGeneratedOnAdd();

                builder.Property(d => d.DepartmentName)
                 .IsRequired()
                 .HasMaxLength(100);
                 
        }
    }
}