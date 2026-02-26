using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Data.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subjects>
    {
        public void Configure(EntityTypeBuilder<Subjects> builder)
        {
            // Primarni ključ
            builder.HasKey(s => s.SubjectId);
            
            builder.Property(s => s.SubjectId)
                   .ValueGeneratedOnAdd();

            builder.Property(s => s.NameSubject)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.DescriptionSubject)
                   .HasMaxLength(500);

            builder.HasOne(s => s.Department)
                   .WithMany(d => d.Subjects)
                   .HasForeignKey(s => s.DepartmentId) 
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}