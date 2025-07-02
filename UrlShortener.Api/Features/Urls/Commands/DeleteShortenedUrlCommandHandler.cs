using MediatR;
using UrlShortener.Api.Mediator.Notifications;

namespace UrlShortener.Api.Features.Urls.Commands;

public class DeleteShortenedUrlCommandHandler : IRequestHandler<DeleteShortenedUrlCommand, bool>
{
    private readonly IUrlRepository _urlRepository;
    private readonly IMediator _mediator;

    public DeleteShortenedUrlCommandHandler(IUrlRepository urlRepository, IMediator mediator)
    {
        _urlRepository = urlRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteShortenedUrlCommand request, CancellationToken cancellationToken)
    {
        var shortenedUrl = await _urlRepository.GetByUniqueId(request.Id, cancellationToken);
        if (shortenedUrl == null)
            return false;


        if (await _urlRepository.DeleteAsync(shortenedUrl, cancellationToken) <= 0) 
            return false;

        await _mediator.Publish(new RemoveCachedUrlRecordNotification(request.Id), cancellationToken);
        return true;
    }
}