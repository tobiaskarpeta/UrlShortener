using MediatR;

public class DeleteShortenedUrlCommandHandler : IRequestHandler<DeleteShortenedUrlCommand, bool>
{
    private readonly IUrlRepository _urlRepository;

    public DeleteShortenedUrlCommandHandler(IUrlRepository urlRepository)
    {
        _urlRepository = urlRepository;
    }

    public async Task<bool> Handle(DeleteShortenedUrlCommand request, CancellationToken cancellationToken)
    {
        var shortenedUrl = await _urlRepository.GetByUniqueId(request.Id, cancellationToken);
        if (shortenedUrl == null)
            return false;

        return await _urlRepository.DeleteAsync(shortenedUrl, cancellationToken) > 0;
    }
}