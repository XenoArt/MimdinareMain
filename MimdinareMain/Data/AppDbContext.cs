using Microsoft.EntityFrameworkCore;
using Mimdinare.Models;
using MimdinareMain.Models;
using System;
using System.Linq;

namespace MimdinareMain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Cashreg> CashRegisters { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.StartTimeUtc)
                    .HasColumnName("StartTime") // Keep old column name
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(b => b.BookingType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(b => b.Duration)
                    .HasConversion(
                        v => v.ToString(@"hh\:mm\:ss"),
                        v => TimeSpan.Parse(v));
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Name);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                // Seed data
                entity.HasData(
                    new Product { Name = "Standard", Price = 50.00m },
                    new Product { Name = "Premium", Price = 80.00m }
                );
            });

            // Purchase configuration
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.FormattedDate).IsRequired();
                entity.Property(p => p.FormattedTime).IsRequired();
            });

            // Cash register configuration
            modelBuilder.Entity<Cashreg>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Cash).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                entity.Property(c => c.Card).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

                // Seed data
                entity.HasData(
                    new Cashreg { Id = 1, Cash = 0, Card = 0, Total = 0 }
                );
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var now = DateTime.UtcNow;
            var georgianNow = Booking.ToGeorgianTime(now);
            var dateStr = georgianNow.ToString("dd.MM.yyyy");
            var timeStr = georgianNow.ToString("HH:mm:ss");

            foreach (var entry in ChangeTracker.Entries<Purchase>()
                .Where(e => e.State == EntityState.Added))
            {
                entry.Entity.FormattedDate = dateStr;
                entry.Entity.FormattedTime = timeStr;
            }
        }
    }
}