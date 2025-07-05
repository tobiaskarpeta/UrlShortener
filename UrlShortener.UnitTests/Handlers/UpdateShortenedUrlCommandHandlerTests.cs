using MediatR;
using Moq;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.UnitTests.Handlers;

public class UpdateShortenedUrlCommandHandlerTests : UnitTestsBase
{
    private Mock<IUrlRepository> _repositoryMock;
    private Mock<IMediator> _mediatorMock;

    private List<string> _notificationCallOrder;

    [SetUp]
    public void SetUp()
    {
        _notificationCallOrder = new List<string>();

        _repositoryMock = new Mock<IUrlRepository>();
        _repositoryMock.Setup(r => r.GetByUniqueId(TestUniqueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShortenedUrl
            {
                Id = 123,
                UniqueId = Guid.Parse(TestUniqueId),
                OriginalUrl = TestUrl,
                ShortCode = TestShortCode
            });


        _mediatorMock = new Mock<IMediator>();

        _mediatorMock.Setup(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), CancellationToken.None))
        .Callback(() => _notificationCallOrder.Add("removed"));

        _mediatorMock.Setup(m => m.Publish(It.IsAny<CacheUpdatedUrlRecordNotification>(), CancellationToken.None))
            .Callback(() => _notificationCallOrder.Add("cached"));

    }

    [Test]
    public async Task Handles_ValidUpdateRequest_ReCachesUrl()
    {
        var request = new UpdateShortenedUrlCommand
        {
            UniqueId = TestUniqueId,
            OriginalUrl = TestUrl2,
            ShortCode = TestShortCode2
        };

        var sut = new UpdateShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(TestUniqueId, CancellationToken.None), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), CancellationToken.None), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<CacheUpdatedUrlRecordNotification>(), CancellationToken.None), Times.Once);

        Assert.That(_notificationCallOrder, Is.EqualTo(new List<string> { "removed", "cached" }));

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ShortenedUrl>());

        Assert.That(result.Id, Is.EqualTo(123));
        Assert.That(result.UniqueId, Is.EqualTo(Guid.Parse(TestUniqueId)));

        Assert.That(result.OriginalUrl, Is.Not.EqualTo(TestUrl));
        Assert.That(result.ShortCode, Is.Not.EqualTo(TestShortCode));

        Assert.That(result.OriginalUrl, Is.EqualTo(request.OriginalUrl));
        Assert.That(result.ShortCode, Is.EqualTo(request.ShortCode));
    }

    [Test]
    public async Task Handles_NonExistingUniqueId_ReturnsNull()
    {
        var request = new UpdateShortenedUrlCommand
        {
            UniqueId = NonExistingUniqueId,
            OriginalUrl = TestUrl2,
            ShortCode = TestShortCode2
        };

        var sut = new UpdateShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(request.UniqueId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<CacheUpdatedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.That(result, Is.Null);
    }
}