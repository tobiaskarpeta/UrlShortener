using MediatR;
using UrlShortener.Api.Models;

public class UpdateShortenedUrlCommand : IRequest<ShortenedUrl?>
{
    public required string UniqueId { get; init; }
    public required string OriginalUrl { get; init; }
    public required string ShortCode { get; init; }
    public DateTime? ExpiresAt { get; init; }
}