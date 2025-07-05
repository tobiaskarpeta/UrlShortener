using MediatR;

namespace UrlShortener.Api.Features.Urls.Commands;

public class DeleteShortenedUrlCommand : IRequest<bool>
{
    public string Id { get; private set; }

    public DeleteShortenedUrlCommand(string id)
    {
        Id = id;
    }
}