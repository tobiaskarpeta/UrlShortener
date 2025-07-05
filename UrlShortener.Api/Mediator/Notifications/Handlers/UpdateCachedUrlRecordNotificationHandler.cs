using MediatR;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Api.Constants;

namespace UrlShortener.Api.Mediator.Notifications.Handlers;

public class UpdateCachedUrlRecordNotificationHandler : INotificationHandler<UpdateCachedUrlRecordNotification>
{
    private readonly IMemoryCache _cache;

    public UpdateCachedUrlRecordNotificationHandler(IMemoryCache cache)
    {
        _cache = cache;
    }


    public Task Handle(UpdateCachedUrlRecordNotification notification, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefixes.Url}:{notification.UpdatedUrlRecord.UniqueId.ToString()}".ToLower();

        _cache.Remove(cacheKey);
        _cache.Set(cacheKey, notification.UpdatedUrlRecord, CacheExpirationTimes.UrlRecord);

        return Task.CompletedTask;
    }
}