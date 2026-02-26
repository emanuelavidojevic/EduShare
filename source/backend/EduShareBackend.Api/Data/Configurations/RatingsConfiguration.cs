using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class  RatingsConfiguration : IEntityTypeConfiguration<Ratings>
    {
        public void Configure(EntityTypeBuilder<Ratings> builder)
        {
             builder.HasKey(s=>s.RatingId);
                builder.Property(s=>s.RatingId)
                .ValueGeneratedOnAdd();

                builder.Property(r => r.ValueForRatings)
               .IsRequired();
             builder.ToTable(t => t.HasCheckConstraint("CK_Rating_Value", "ValueForRatings >= 1 AND ValueForRatings <= 5"));
            builder.HasOne(u => u.User)
                   .WithMany(s => s.Rating)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Material)
                   .WithMany(s => s.Rating)
                   .HasForeignKey(m => m.MaterialId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
