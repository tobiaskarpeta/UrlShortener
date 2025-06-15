using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure indexes and constraints
            modelBuilder.Entity<ShortenedUrl>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();
                
            modelBuilder.Entity<ShortenedUrl>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
                
            modelBuilder.Entity<ShortenedUrl>()
                .Property(u => u.AccessCount)
                .HasDefaultValue(0);
        }
    }
}
