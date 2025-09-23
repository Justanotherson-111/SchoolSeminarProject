using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.DataBase
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ImageRecord> ImageRecords { get; set; }
        public DbSet<ExtractedText> ExtractedTexts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasMany(u => u.Images)
            .WithOne(i => i.Owner)
            .HasForeignKey(i => i.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ImageRecord>().HasMany(i => i.ExtractedTexts)
            .WithOne(t => t.ImageRecord)
            .HasForeignKey(t => t.ImageRecordId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}