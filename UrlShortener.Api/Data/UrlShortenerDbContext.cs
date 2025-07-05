using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Data
{
    public class UrlShortenerDbContext : DbContext, IUrlShortenerContext
    {
        public UrlShortenerDbContext()
        {
        }

        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

    public interface IUrlShortenerContext
    {
        DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    }
}
