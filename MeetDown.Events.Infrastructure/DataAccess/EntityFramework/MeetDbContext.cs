using MeetDown.Events.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MeetDown.Events.Infrastructure.DataAccess.EntityFramework
{
    public class MeetDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MeetDbContext(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Meet> Meets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();

            // Group entity configuration
            modelBuilder.Entity<Group>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Group>()
                .Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .Property(g => g.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<Group>()
                .Property(g => g.Location)
                .HasMaxLength(100);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Meets)
                .WithOne(e => e.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Meet entity configuration
            modelBuilder.Entity<Meet>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Meet>()
                .Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Meet>()
                .Property(g => g.Description)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Meet>()
                .Property(g => g.DateOfMeet)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
