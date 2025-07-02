using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace UrlShortener.Api.Mediator.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;

    public CachingBehavior(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableRequest<TResponse> cacheableRequest)
        {
            return await next(cancellationToken);
        }

        var cacheKey = cacheableRequest.CacheKey.ToLower();

        if (_cache.TryGetValue(cacheKey, out var cacheResult) && cacheResult is TResponse response)
        {
            return response;
        }

        response = await next(cancellationToken);
        
        _cache.Set(cacheKey, response, cacheableRequest.Expiration);
        return response;
    }
}