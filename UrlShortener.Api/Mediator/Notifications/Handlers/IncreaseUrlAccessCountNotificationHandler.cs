using MediatR;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Mediator.Notifications.Handlers;

public class IncreaseUrlAccessCountNotificationHandler : INotificationHandler<IncreaseUrlAccessCountNotification>
{
    private readonly IUrlRepository _repository;
    private readonly IMediator _mediator;

    public IncreaseUrlAccessCountNotificationHandler(IUrlRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task Handle(IncreaseUrlAccessCountNotification notification, CancellationToken cancellationToken)
    {
        notification.Url.AccessCount++;

        await _repository.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new RemoveCachedUrlRecordNotification(notification.Url.UniqueId.ToString()), cancellationToken);
    }
}