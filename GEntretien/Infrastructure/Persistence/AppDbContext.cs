using Microsoft.EntityFrameworkCore;
using GEntretien.Domain.Entities;

namespace GEntretien.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Equipment> Equipments => Set<Equipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Equipment>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Name).IsRequired();
            });
        }
    }
}
