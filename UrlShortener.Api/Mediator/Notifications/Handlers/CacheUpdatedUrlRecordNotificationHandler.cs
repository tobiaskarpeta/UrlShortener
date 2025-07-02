using MediatR;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Api.Constants;

namespace UrlShortener.Api.Mediator.Notifications.Handlers;

public class CacheUpdatedUrlRecordNotificationHandler : INotificationHandler<CacheUpdatedUrlRecordNotification>
{
    private readonly IMemoryCache _memoryCache;

    public CacheUpdatedUrlRecordNotificationHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task Handle(CacheUpdatedUrlRecordNotification notification, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefixes.Url}:{notification.UpdatedUrlRecord.UniqueId.ToString()}".ToLower();

        _memoryCache.Remove(cacheKey); // Just to be sure. It should not throw any error if the key does not exist.
        _memoryCache.Set(cacheKey, notification.UpdatedUrlRecord, CacheExpirationTimes.UrlRecord);

        return Task.CompletedTask;
    }
}