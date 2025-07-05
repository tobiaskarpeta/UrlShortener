using Moq;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.UnitTests.Handlers;

[TestFixture]
public class ShortenUrlCommandHandlerTests : UnitTestsBase
{
    private Mock<IUrlRepository> _repositoryMock;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IUrlRepository>();
    }

    [Test]
    public async Task Handle_ValidRequestWithCustomCode_CreatesShortenedUrl()
    {
        var request = new ShortenUrlCommand
        {
            Url = TestUrl,
            CustomCode = TestShortCode
        };

        var sut = new ShortenUrlCommandHandler(_repositoryMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.ShortCodeExistsAsync(TestShortCode, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(result, It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ShortenedUrl>());

        AssertCreatedShortenedUrl(result, request);
    }

    [Test]
    public async Task Handle_ValidRequestWithoutCustomCode_CreatesShortenedUrl()
    {
        var request = new ShortenUrlCommand
        {
            Url = TestUrl,
        };

        var sut = new ShortenUrlCommandHandler(_repositoryMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(result, It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ShortenedUrl>());

        AssertCreatedShortenedUrl(result, request);
    }

    [Test]
    public void Handle_ExistingCustomCode_ThrowsException()
    {
        var request = new ShortenUrlCommand
        {
            Url = TestUrl,
            CustomCode = TestShortCode
        };

        _repositoryMock.Setup(r => r.ShortCodeExistsAsync(request.CustomCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new ShortenUrlCommandHandler(_repositoryMock.Object);
        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(request, CancellationToken.None));

        _repositoryMock.Verify(r => r.ShortCodeExistsAsync(request.CustomCode, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ShortenedUrl>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.That(exception.Message, Is.EqualTo($"Custom short code is already taken. Please choose a different one."));
    }


    private static void AssertCreatedShortenedUrl(ShortenedUrl result, ShortenUrlCommand request)
    {
        var customCodeEntered = !string.IsNullOrEmpty(request.CustomCode);

        Assert.That(result.OriginalUrl, Is.EqualTo(request.Url));

        if (customCodeEntered)
            Assert.That(result.ShortCode, Is.EqualTo(request.CustomCode));
        else
            Assert.That(result.ShortCode, Is.Not.Empty);

        Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(result.CreatedBy, Is.Null);
        Assert.That(result.ExpiresAt, Is.Null);
    }
}