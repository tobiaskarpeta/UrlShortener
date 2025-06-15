using UrlShortener.Api.Models;

public interface IUrlRepository
{
    Task AddAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken);
    Task<ShortenedUrl?> GetByShortCode(string shortCode, CancellationToken cancellationToken);
    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken);
}