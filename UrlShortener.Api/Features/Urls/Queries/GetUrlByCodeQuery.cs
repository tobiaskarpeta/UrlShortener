using MediatR;

namespace UrlShortener.Api.Features.Urls.Queries
{
    public record GetUrlByCodeQuery : IRequest<string?>
    {
        public required string ShortCode { get; init; }
    }
}
