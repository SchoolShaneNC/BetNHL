using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BetNHL_Web_Api.Data
{
    public class BetNHLContext : IdentityDbContext<ApplicationUser>
    {
        public BetNHLContext(DbContextOptions<BetNHLContext> options)
            : base(options)
        {
        }
        public DbSet<Parlay> Parlays { get; set; }

        public DbSet<ParlayLeg> ParlayLegs { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER -> PARLAYS
            modelBuilder.Entity<Parlay>()
                .HasOne(p => p.User)
                .WithMany(u => u.Parlays)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PARLAY -> LEGS
            modelBuilder.Entity<ParlayLeg>()
                .HasOne(l => l.Parlay)
                .WithMany(p => p.Legs)
                .HasForeignKey(l => l.ParlayId)
                .OnDelete(DeleteBehavior.Cascade);

            // ENUM STRING CONVERSIONS
            modelBuilder.Entity<ParlayLeg>()
                .Property(p => p.Category)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ParlayLeg>()
                .Property(p => p.Metric)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ParlayLeg>()
                .Property(p => p.Condition)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ParlayLeg>()
                .Property(p => p.Context)
                .HasConversion<string>()
                .HasMaxLength(20);

            // DECIMAL PRECISION
            modelBuilder.Entity<Parlay>()
                .Property(p => p.Stake)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Parlay>()
                .Property(p => p.CombinedOdds)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Parlay>()
                .Property(p => p.PotentialPayout)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ParlayLeg>()
                .Property(l => l.Odds)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ParlayLeg>()
                .Property(l => l.Line)
                .HasPrecision(18, 2);

            // INDEXES
            modelBuilder.Entity<Parlay>()
                .HasIndex(p => p.UserId);

            modelBuilder.Entity<Parlay>()
                .HasIndex(p => p.Won);

            modelBuilder.Entity<ParlayLeg>()
                .HasIndex(l => l.GameId);

            modelBuilder.Entity<ParlayLeg>()
                .HasIndex(l => l.ParlayId);
        }
    }
}