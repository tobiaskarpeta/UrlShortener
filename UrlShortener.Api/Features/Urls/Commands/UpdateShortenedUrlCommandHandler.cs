using AutoMapper;
using MediatR;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Features.Urls.Commands;

public class UpdateShortenedUrlCommandHandler : IRequestHandler<UpdateShortenedUrlCommand, ShortenedUrl?>
{
    private readonly IUrlRepository _repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UpdateShortenedUrlCommandHandler(IUrlRepository repository, IMediator mediator, IMapper mapper)
    {
        _repository = repository;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ShortenedUrl?> Handle(UpdateShortenedUrlCommand request, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByUniqueId(request.UniqueId, cancellationToken);
        if (existingUrl == null)
        {
            return null;
        }

        if (!string.Equals(request.ShortCode, existingUrl.ShortCode) && await _repository.ShortCodeExistsAsync(request.ShortCode, cancellationToken))
        {
            throw new InvalidOperationException("Selected custom code is taken. Choose different one.");
        }

        //TODO: Reset access count if originalUrl or shortCode is changed???
        _mapper.Map(request, existingUrl);

        await _repository.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new UpdateCachedUrlRecordNotification(existingUrl), cancellationToken);

        return existingUrl;
    }
}