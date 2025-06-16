using MediatR;
using UrlShortener.Api.Models;

public class GetShortenedUrlQuery : IRequest<ShortenedUrl?>
{
    public string UniqueId { get; private set; }

    public GetShortenedUrlQuery(string uniqueId)
    {
        UniqueId = uniqueId;
    }
}