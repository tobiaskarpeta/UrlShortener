using MediatR;
using Moq;
using UrlShortener.Api.Features.Urls.Queries;
using UrlShortener.Api.Mediator.Notifications;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.UnitTests.Handlers;

public class GetUrlByCodeQueryHandlerTests : UnitTestsBase
{
    private List<ShortenedUrl> _urls;

    private Mock<IUrlRepository> _repositoryMock;
    private Mock<IMediator> _mediatorMock;

    [SetUp]
    public void SetUp()
    {
        _urls = [
            new ShortenedUrl
            {
                Id = 1,
                UniqueId = Guid.Parse(TestUniqueId),
                OriginalUrl = TestUrl,
                ShortCode = TestShortCode,
            },

            new ShortenedUrl
            {
                Id = 2,
                UniqueId = Guid.Parse(TestUniqueId2),
                OriginalUrl = TestUrl2,
                ShortCode = TestShortCode2,
            }
        ];

        _repositoryMock = new Mock<IUrlRepository>();
        _repositoryMock.Setup(r => r.GetByShortCode(TestShortCode, It.IsAny<CancellationToken>())).ReturnsAsync(_urls[0]);
        _repositoryMock.Setup(r => r.GetByShortCode(TestShortCode2, It.IsAny<CancellationToken>())).ReturnsAsync(_urls[1]);

        _mediatorMock = new Mock<IMediator>();
    }

    [TestCase(TestShortCode)]
    [TestCase(TestShortCode2)]
    public async Task Handle_ExistingShortCode_ReturnsOriginalUrl(string shortCode)
    {
        var request = new GetUrlByCodeQuery(shortCode);
        var expectedResult = _urls.First(u => u.ShortCode == shortCode).OriginalUrl;

        var sut = new GetUrlByCodeQueryHandler(_repositoryMock.Object, _mediatorMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByShortCode(shortCode, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<IncreaseUrlAccessCountNotification>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task Handle_NonExistingShortCode_ReturnsNull()
    {
        var request = new GetUrlByCodeQuery(NonExistingUniqueId);

        var sut = new GetUrlByCodeQueryHandler(_repositoryMock.Object, _mediatorMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByShortCode(NonExistingUniqueId, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RemoveCachedUrlRecordNotification>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.That(result, Is.Null);
    }
}