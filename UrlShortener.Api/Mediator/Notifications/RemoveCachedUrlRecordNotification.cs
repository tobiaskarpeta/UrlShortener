using MediatR;

namespace UrlShortener.Api.Mediator.Notifications;

public class RemoveCachedUrlRecordNotification(string uniqueId) : INotification
{
    public string UniqueId { get; } = uniqueId;
}