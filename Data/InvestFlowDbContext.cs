using InvestFlowAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace InvestFlowAPI.Data
{
    public class InvestFlowDbContext : DbContext
    {
        public InvestFlowDbContext(
            DbContextOptions<InvestFlowDbContext> options)
            : base(options)
        {
        }
        // ==========================================
        // DATABASE TABLES
        // ==========================================
        public DbSet<User> Users { get; set; }
        public DbSet<InvestmentPlan> InvestmentPlans { get; set; }
        public DbSet<Investment> Investments { get; set; }
        // ==========================================
        // MODEL CONFIGURATION
        // ==========================================
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // ==========================================
            // USER
            // ==========================================
            modelBuilder.Entity<User>()
                .Property(u => u.AccountBalance)
                .HasPrecision(18, 2);
            // ==========================================
            // INVESTMENT PLAN
            // ==========================================
            modelBuilder.Entity<InvestmentPlan>()
                .Property(p => p.ProfitRate)
                .HasPrecision(5, 2);
            modelBuilder.Entity<InvestmentPlan>()
                .Property(p => p.MinimumAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<InvestmentPlan>()
                .Property(p => p.MaximumAmount)
                .HasPrecision(18, 2);
            // ==========================================
            // INVESTMENT
            // ==========================================
            modelBuilder.Entity<Investment>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Investment>()
                .Property(i => i.ProfitAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Investment>()
                .Property(i => i.TotalReturn)
                .HasPrecision(18, 2);
            // ==========================================
            // USER → INVESTMENT
            // ==========================================
            modelBuilder.Entity<Investment>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            // ==========================================
            // INVESTMENT PLAN → INVESTMENT
            // ==========================================
            modelBuilder.Entity<Investment>()
                .HasOne(i => i.InvestmentPlan)
                .WithMany()
                .HasForeignKey(i => i.PlanID)
                .OnDelete(DeleteBehavior.Restrict);
            // ==========================================
            // FINISH MODEL CONFIGURATION
            // ==========================================
            base.OnModelCreating(modelBuilder);
        }
    }
}