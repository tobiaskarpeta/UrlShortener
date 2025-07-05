using MediatR;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Mediator.Notifications;

public class IncreaseUrlAccessCountNotification : INotification
{
    public ShortenedUrl Url { get; }

    public IncreaseUrlAccessCountNotification(ShortenedUrl url)
    {
        Url = url;
    }
}