using MediatR;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Api.Constants;

namespace UrlShortener.Api.Mediator.Notifications.Handlers;

public class RemoveCachedUrlRecordHandler : INotificationHandler<RemoveCachedUrlRecordNotification>
{
    private readonly IMemoryCache _cache;

    public RemoveCachedUrlRecordHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task Handle(RemoveCachedUrlRecordNotification notification, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CachePrefixes.Url}:{notification.UniqueId}";
        _cache.Remove(cacheKey.ToLower());

        return Task.CompletedTask;
    }
}