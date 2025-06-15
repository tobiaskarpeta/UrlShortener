using MediatR;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Urls.Commands
{
    public record ShortenUrlCommand : IRequest<ShortenedUrl>
    {
        public required string Url { get; init; }
        public string? CustomCode { get; init; }
    }
}
