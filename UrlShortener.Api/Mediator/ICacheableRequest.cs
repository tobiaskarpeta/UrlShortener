using MediatR;

namespace UrlShortener.Api.Mediator;

public interface ICacheableRequest<out TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }
    TimeSpan Expiration { get; }
}