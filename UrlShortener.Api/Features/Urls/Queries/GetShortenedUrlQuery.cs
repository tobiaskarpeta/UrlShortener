using UrlShortener.Api.Constants;
using UrlShortener.Api.Mediator;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Urls.Queries;

public class GetShortenedUrlQuery(string uniqueId) : ICacheableRequest<ShortenedUrl?>
{
    public string UniqueId { get; } = uniqueId;

    public string CacheKey => $"{CachePrefixes.Url}:{UniqueId}";
    public TimeSpan Expiration => CacheExpirationTimes.UrlRecord;
}