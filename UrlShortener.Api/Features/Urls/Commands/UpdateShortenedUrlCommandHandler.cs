using MediatR;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Features.Urls.Commands;

public class UpdateShortenedUrlCommandHandler : IRequestHandler<UpdateShortenedUrlCommand, ShortenedUrl?>
{
    private readonly IUrlRepository _repository;
    private readonly IMediator _mediator;

    public UpdateShortenedUrlCommandHandler(IUrlRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task<ShortenedUrl?> Handle(UpdateShortenedUrlCommand request, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByUniqueId(request.UniqueId, cancellationToken);
        if (existingUrl == null)
        {
            return null;
        }

        //TODO: Validate the request data (e.g., check if the short code is unique, if the original URL is valid, etc.)
        //TODO: Reset access count if originalUrl or shortCode is changed???
        //TODO: Use AutoMapper for mapping
        existingUrl.OriginalUrl = request.OriginalUrl;
        existingUrl.ShortCode = request.ShortCode;
        existingUrl.ExpiresAt = request.ExpiresAt;

        await _repository.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new UpdateCachedUrlRecordNotification(existingUrl), cancellationToken);

        return existingUrl;
    }
}