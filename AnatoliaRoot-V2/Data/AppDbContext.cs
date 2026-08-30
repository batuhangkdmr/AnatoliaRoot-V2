using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Models;

namespace AnatoliaRoot_V2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<GoldPrice> GoldPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NormalizedUsername).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.SecurityStamp).IsRequired().HasMaxLength(36);
                entity.HasIndex(e => e.NormalizedUsername).IsUnique();
            });

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Categories_Root_Name")
                    .HasFilter("[ParentCategoryId] IS NULL");
                entity.HasIndex(e => new { e.ParentCategoryId, e.Name }).IsUnique();
                entity.HasOne(e => e.ParentCategory)
                    .WithMany(e => e.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(table => table.HasCheckConstraint(
                    "CK_Categories_NotSelfParent",
                    "[ParentCategoryId] IS NULL OR [ParentCategoryId] <> [Id]"));
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(1000);
                
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.Property(e => e.UsdRate).HasPrecision(18, 8);
                entity.Property(e => e.EurRate).HasPrecision(18, 8);
                entity.Property(e => e.UsdTry).HasPrecision(18, 6);
                entity.Property(e => e.EurTry).HasPrecision(18, 6);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.SourceTimestamp)
                    .IsUnique()
                    .HasFilter("[SourceTimestamp] IS NOT NULL");
                entity.ToTable(table => table.HasCheckConstraint(
                    "CK_ExchangeRates_PositiveValues",
                    "[UsdRate] > 0 AND [EurRate] > 0 AND [UsdTry] > 0 AND [EurTry] > 0"));
            });

            modelBuilder.Entity<GoldPrice>(entity =>
            {
                entity.Property(e => e.GramGold).HasPrecision(18, 4);
                entity.Property(e => e.QuarterGold).HasPrecision(18, 4);
                entity.Property(e => e.HalfGold).HasPrecision(18, 4);
                entity.Property(e => e.ChangeRate).HasPrecision(9, 4);
                entity.Property(e => e.DayHigh).HasPrecision(18, 4);
                entity.Property(e => e.DayLow).HasPrecision(18, 4);
                entity.Property(e => e.PrevClose).HasPrecision(18, 4);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Timestamp).IsUnique();
                entity.ToTable(table => table.HasCheckConstraint(
                    "CK_GoldPrices_ValidValues",
                    "[GramGold] > 0 AND [QuarterGold] > 0 AND [HalfGold] > 0 AND [DayHigh] > 0 AND [DayLow] > 0 AND [DayLow] <= [DayHigh] AND [PrevClose] > 0"));
            });
        }
    }
}
