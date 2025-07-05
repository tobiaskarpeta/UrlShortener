using MediatR;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Features.Urls.Queries;

public class GetShortenedUrlQueryHandler : IRequestHandler<GetShortenedUrlQuery, ShortenedUrl?>
{
    private readonly IUrlRepository _repository;

    public GetShortenedUrlQueryHandler(IUrlRepository repository)
    {
        _repository = repository;
    }

    public Task<ShortenedUrl?> Handle(GetShortenedUrlQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetByUniqueId(request.UniqueId, cancellationToken);
    }
}