using MediatR;

namespace UrlShortener.Api.Features.Urls.Queries
{
    public class GetUrlByCodeQueryHandler : IRequestHandler<GetUrlByCodeQuery, string?>
    {
        private readonly IUrlRepository _repository;

        public GetUrlByCodeQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<string?> Handle(GetUrlByCodeQuery request, CancellationToken cancellationToken)
        {
            var shortenedUrlRecord = await _repository.GetByShortCode(request.ShortCode, cancellationToken);
            if (shortenedUrlRecord == null)
            {
                return null;
            }

            shortenedUrlRecord.AccessCount++;
            await _repository.SaveChangesAsync(cancellationToken);
            
            return shortenedUrlRecord.OriginalUrl;
        }
    }
}
