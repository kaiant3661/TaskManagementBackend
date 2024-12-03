using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Userr> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Mapping the Userr entity to the 'Userr' table in the database
            modelBuilder.Entity<Userr>().ToTable("Userr");

            // Define the primary key explicitly (EF Core can infer this, but for clarity):
            modelBuilder.Entity<Userr>().HasKey(u => u.UserId);

            // Optionally configure other aspects of the model, such as relationships, constraints, etc.

            modelBuilder.Entity<Models.Task>()
                .Property(t => t.Status)
                .HasConversion<string>(); // Storing Status as string (Pending, InProgress, Completed)

            modelBuilder.Entity<Models.Task>()
                .Property(t => t.Priority)
                .HasConversion<string>(); // Storing Priority as string (Low, Medium, High)


            modelBuilder.Entity<Models.Task>()
    .HasOne(t => t.AssignedToUser)
    .WithMany()
    .HasForeignKey(t => t.AssignedToUserId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Role>()
               .HasKey(r => r.RoleId);

            modelBuilder.Entity<Userr>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<AuditLog>()
                .ToTable("AuditLogs")
                .HasKey(a => a.AuditLogId);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.PerformedByUser)
                .WithMany() // One user can perform many actions
                .HasForeignKey(a => a.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        }





    }



}
