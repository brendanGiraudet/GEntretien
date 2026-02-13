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
        public DbSet<Intervention> Interventions => Set<Intervention>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Equipment>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Intervention>(b =>
            {
                b.HasKey(i => i.Id);
                b.Property(i => i.Date).IsRequired();
                b.Property(i => i.Description).IsRequired();
                b.Property(i => i.Status).IsRequired();
                b.HasOne(i => i.Equipment)
                    .WithMany(e => e.Interventions)
                    .HasForeignKey(i => i.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
