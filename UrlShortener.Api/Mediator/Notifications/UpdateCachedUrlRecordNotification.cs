using MediatR;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Mediator.Notifications;

public class UpdateCachedUrlRecordNotification : INotification
{
    public ShortenedUrl UpdatedUrlRecord { get; }

    public UpdateCachedUrlRecordNotification(ShortenedUrl updatedUrlRecord)
    {
        UpdatedUrlRecord = updatedUrlRecord ?? throw new ArgumentNullException(nameof(updatedUrlRecord));
    }

}