using MediatR;

namespace UrlShortener.Api.Features.Urls.Queries
{
    public record GetUrlByCodeQuery : IRequest<string?>
    {
        public string ShortCode { get; }

        public GetUrlByCodeQuery(string shortCode)
        {
            ShortCode = shortCode;
        }
    }
}
