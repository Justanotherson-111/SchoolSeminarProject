using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<TextFile> TextFiles => Set<TextFile>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OcrJob> OcrJobs => Set<OcrJob>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ Images
            modelBuilder.Entity<User>()
                .HasMany(u => u.Images)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // User ↔ RefreshTokens
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Image ↔ TextFiles
            modelBuilder.Entity<Image>()
                .HasMany(i => i.TextFiles)
                .WithOne(t => t.Image)
                .HasForeignKey(t => t.ImageId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Enforce string lengths and required
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<RefreshToken>()
                .Property(r => r.Token)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .Property(i => i.FileName)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .Property(i => i.OriginalFileName)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .Property(i => i.RelativePath)
                .IsRequired();

            modelBuilder.Entity<TextFile>()
                .Property(t => t.TxtFilePath)
                .IsRequired();

            modelBuilder.Entity<TextFile>()
                .Property(t => t.Language)
                .HasMaxLength(10)
                .IsRequired();

            // Unique constraint on Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
