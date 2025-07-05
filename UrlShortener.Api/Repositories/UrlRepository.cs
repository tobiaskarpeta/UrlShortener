using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Repositories;

public class UrlRepository : RepositoryBase, IUrlRepository
{
    public UrlRepository(UrlShortenerDbContext context) : base(context)
    {
    }

    public Task AddAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken)
    {
        _context.ShortenedUrls.Add(shortenedUrl);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken)
    {
        _context.ShortenedUrls.Remove(shortenedUrl);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ShortenedUrl?> GetByShortCode(string shortCode, CancellationToken cancellationToken)
    {
        return _context.ShortenedUrls.FirstOrDefaultAsync(url => url.ShortCode == shortCode);
    }

    public Task<ShortenedUrl?> GetByUniqueId(string uniqueId, CancellationToken cancellationToken)
    {
        return _context.ShortenedUrls.FirstOrDefaultAsync(url => url.UniqueId.ToString() == uniqueId, cancellationToken);
    }

    public Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken)
    {
        return _context.ShortenedUrls.AnyAsync(url => url.ShortCode == shortCode, cancellationToken);
    }
}