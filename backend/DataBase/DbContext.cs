using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.DataBase
{
    public class AppDbContext : IdentityDbContext<User>
    {
        // Constructor for runtime DI
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Optional: parameterless constructor for design-time tools (not required if using IDesignTimeDbContextFactory)
        // public AppDbContext() { }

        public DbSet<ImageRecord> ImageRecords { get; set; }
        public DbSet<ExtractedText> ExtractedTexts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User -> Images relationship
            builder.Entity<User>()
                .HasMany(u => u.Images)
                .WithOne(i => i.Owner)
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ImageRecord -> ExtractedTexts relationship
            builder.Entity<ImageRecord>()
                .HasMany(i => i.ExtractedTexts)
                .WithOne(t => t.ImageRecord)
                .HasForeignKey(t => t.ImageRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> RefreshTokens relationship
            builder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
