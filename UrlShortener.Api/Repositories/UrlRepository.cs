using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

public class UrlRepository : IUrlRepository
{
    private readonly UrlShortenerDbContext _context;

    public UrlRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken)
    {
        _context.ShortenedUrls.Add(shortenedUrl);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ShortenedUrl?> GetByShortCode(string shortCode, CancellationToken cancellationToken)
    {
        return _context.ShortenedUrls.FirstOrDefaultAsync(url => url.ShortCode == shortCode);
    }

    public Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken)
    {
        return _context.ShortenedUrls.AnyAsync(url => url.ShortCode == shortCode, cancellationToken);
    }
}