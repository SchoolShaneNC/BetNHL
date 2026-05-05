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

        public DbSet<Bet> Bets { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // REQUIRED

            modelBuilder.Entity<Bet>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}