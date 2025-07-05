using MediatR;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Features.Urls.Queries
{
    public class GetUrlByCodeQueryHandler : IRequestHandler<GetUrlByCodeQuery, string?>
    {
        private readonly IUrlRepository _repository;
        private readonly IMediator _mediator;

        public GetUrlByCodeQueryHandler(IUrlRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<string?> Handle(GetUrlByCodeQuery request, CancellationToken cancellationToken)
        {
            var shortenedUrlRecord = await _repository.GetByShortCode(request.ShortCode, cancellationToken);
            if (shortenedUrlRecord == null)
            {
                return null;
            }

            //TODO: Maybe it would be better to publish a notification here to update the record
            shortenedUrlRecord.AccessCount++;

            await _repository.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new RemoveCachedUrlRecordNotification(shortenedUrlRecord.UniqueId.ToString()), cancellationToken);

            return shortenedUrlRecord.OriginalUrl;
        }
    }
}
