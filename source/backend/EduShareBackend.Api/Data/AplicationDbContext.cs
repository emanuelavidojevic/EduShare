using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Data
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options)
            : base(options)
        {}
        public DbSet<Users>Users{get;set;}
         public DbSet<Materials>Materials{get;set;}

        public DbSet<Comments>Comments{get;set;}

        public DbSet<Departments>Departments{get;set;}

        public DbSet<Ratings>Ratings{get;set;}

        public DbSet<Subjects>Subjects{get;set;}
         public DbSet<MaterialData>MaterialData{get;set;}
        public DbSet<Roles>Roles{get;set;}
         public DbSet<UserRoles>UserRoles{get;set;}
         public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AplicationDbContext).Assembly);
        }

    

    }
}