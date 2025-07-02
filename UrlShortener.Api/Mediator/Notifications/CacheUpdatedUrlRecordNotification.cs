using MediatR;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Mediator.Notifications;

public class CacheUpdatedUrlRecordNotification(ShortenedUrl updatedUrlRecord) : INotification
{
    public ShortenedUrl UpdatedUrlRecord { get; } = updatedUrlRecord;
}