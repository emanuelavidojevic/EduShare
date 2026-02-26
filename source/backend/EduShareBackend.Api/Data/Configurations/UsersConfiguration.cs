using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduShareBackend.Api.Data.Configurations
{
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
            {
                builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).ValueGeneratedOnAdd();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Password).IsRequired();
        builder.Property(u => u.DateRegistration).HasDefaultValueSql("GETDATE()");

        builder.HasMany(u => u.Roles)
               .WithMany(r => r.User)
               .UsingEntity<UserRoles>(
                   j => j.HasOne(ur => ur.Role)
                         .WithMany(r => r.UserRoles)
                         .HasForeignKey(ur => ur.RoleId),
                   j => j.HasOne(ur => ur.User)
                         .WithMany(u => u.UserRoles)
                         .HasForeignKey(ur => ur.UserId)
               );
        builder.HasOne(u => u.Department)
               .WithMany(d => d.Users)
               .HasForeignKey(u => u.DepartmentId)
               .OnDelete(DeleteBehavior.SetNull);
            }
        }
    }
            
}
