using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class  CommentsConfiguration : IEntityTypeConfiguration<Comments>
    {
        public void Configure(EntityTypeBuilder<Comments> builder)
        {
            builder.HasKey(c => c.CommentId);
              builder.Property(c=>c.CommentId)
                .ValueGeneratedOnAdd();
                
                builder.Property(c => c.TextOfComments)
                .IsRequired()
                .HasMaxLength(1000);
                
                builder.Property(c => c.DateComments)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
                
                builder.HasOne(c => c.User)
                   .WithMany(u => u.Comment)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
                   
                   builder.HasOne(c => c.Material)
                   .WithMany(m => m.Comment)
                   .HasForeignKey(c => c.MaterialId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
