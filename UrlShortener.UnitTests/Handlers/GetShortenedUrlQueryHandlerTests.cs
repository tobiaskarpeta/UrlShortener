using Moq;
using UrlShortener.Api.Features.Urls.Queries;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.UnitTests.Handlers;

[TestFixture]
public class GetShortenedUrlQueryHandlerTests : UnitTestsBase
{
    private List<ShortenedUrl> _urls;
    private Mock<IUrlRepository> _repositoryMock;

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
        _repositoryMock.Setup(r => r.GetByUniqueId(TestUniqueId, It.IsAny<CancellationToken>())).ReturnsAsync(_urls[0]);
        _repositoryMock.Setup(r => r.GetByUniqueId(TestUniqueId2, It.IsAny<CancellationToken>())).ReturnsAsync(_urls[1]);
    }

    [TestCase(TestUniqueId)]
    [TestCase(TestUniqueId2)]
    public async Task Handle_ExistingUniqueId_ReturnsObject(string urlUniqueId)
    {
        var request = new GetShortenedUrlQuery(urlUniqueId);
        var expectedResult = _urls.First(u => u.UniqueId.ToString() == urlUniqueId);

        var sut = new GetShortenedUrlQueryHandler(_repositoryMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(urlUniqueId, It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task Handle_NonExistingUniqueId_ReturnsNull()
    {
        var request = new GetShortenedUrlQuery(NonExistingUniqueId);

        var sut = new GetShortenedUrlQueryHandler(_repositoryMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(NonExistingUniqueId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(result, Is.Null);
    }
}