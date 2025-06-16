using UrlShortener.Api.Models;

public interface IUrlRepository : IRepository
{
    Task AddAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken);
    Task<int> DeleteAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken);
    Task<ShortenedUrl?> GetByUniqueId(string uniqueId, CancellationToken cancellationToken);
    Task<ShortenedUrl?> GetByShortCode(string shortCode, CancellationToken cancellationToken);
    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken);
}