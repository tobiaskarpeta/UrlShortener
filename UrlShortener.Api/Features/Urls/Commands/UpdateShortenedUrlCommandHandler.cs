using MediatR;
using UrlShortener.Api.Models;

public class UpdateShortenedUrlCommandHandler : IRequestHandler<UpdateShortenedUrlCommand, ShortenedUrl?>
{
    private readonly IUrlRepository _repository;

    public UpdateShortenedUrlCommandHandler(IUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShortenedUrl?> Handle(UpdateShortenedUrlCommand request, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByUniqueId(request.UniqueId, cancellationToken);
        if (existingUrl == null)
        {
            return null;
        }

        existingUrl.OriginalUrl = request.OriginalUrl;
        existingUrl.ShortCode = request.ShortCode;
        existingUrl.ExpiresAt = request.ExpiresAt;

        await _repository.SaveChangesAsync(cancellationToken);
        return existingUrl;
    }
}