using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduShareBackend.Api.Data.Configurations
{
    
    namespace EduShareBackend.Api.Data.Configurations
{
    public class RolesConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
            {
                 
                builder.HasKey(r => r.RoleId);
                builder.Property(r => r.RoleId).ValueGeneratedOnAdd();
                builder.Property(r => r.RoleName).IsRequired().HasMaxLength(30);
            }
    }
}
}