using MediatR;
using Moq;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.UnitTests.Handlers;

[TestFixture]
public class DeleteShortenedUrlCommandHandlerTests : UnitTestsBase
{
    private Mock<IUrlRepository >_repositoryMock;
    private Mock<IMediator> _mediatorMock;

    [SetUp]
    public void SetUp()
    {
        var urls = new List<ShortenedUrl>
        {
            new()
            {
                Id = 1,
                UniqueId = Guid.Parse(TestUniqueId),
                OriginalUrl = TestUrl,
                ShortCode = TestShortCode,
            },
            new()
            {
                Id = 2,
                UniqueId = Guid.Parse(TestUniqueId2),
                OriginalUrl = TestUrl2,
                ShortCode = TestShortCode2,
            }
        };

        _repositoryMock = new Mock<IUrlRepository>();
        _repositoryMock.Setup(r => r.GetByUniqueId(TestUniqueId, It.IsAny<CancellationToken>())).ReturnsAsync(urls[0]);
        _repositoryMock.Setup(r => r.GetByUniqueId(TestUniqueId2, It.IsAny<CancellationToken>())).ReturnsAsync(urls[1]);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mediatorMock = new Mock<IMediator>();
    }

    [TestCase(TestUniqueId)]
    [TestCase(TestUniqueId2)]
    public async Task Handle_ExistingUniqueId_DeletesUrl_And_RemovesItFromCache(string urlToDeleteId)
    {
        var request = new DeleteShortenedUrlCommand(urlToDeleteId);

        var sut = new DeleteShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
        var deleted = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(urlToDeleteId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>()), Times.Once);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(deleted, Is.True);
    }

    [Test]
    public async Task Handle_NonExistingUniqueId_ReturnsFalse()
    {
        const string nonExistingId = NonExistingUniqueId;
        var request = new DeleteShortenedUrlCommand(nonExistingId);

        var sut = new DeleteShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
        var deleted = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(nonExistingId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>()), Times.Never);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.That(deleted, Is.False);
    }

    [Test]
    public async Task Handle_RepositoryDeleteFails_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var request = new DeleteShortenedUrlCommand(TestUniqueId);

        var sut = new DeleteShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
        var deleted = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(TestUniqueId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>()), Times.Once);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.That(deleted, Is.False);
    }
}