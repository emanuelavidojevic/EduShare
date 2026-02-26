using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduShareBackend.Api.Data.Configurations
{
    
    namespace EduShareBackend.Api.Data.Configurations
{
public class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
     builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}
}
}