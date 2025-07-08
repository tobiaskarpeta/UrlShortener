using AutoMapper;
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
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
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
        _mediatorMock.Setup(m => m.Publish(It.IsAny<UpdateCachedUrlRecordNotification>(), CancellationToken.None));

        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateShortenedUrlCommand>(), It.IsAny<ShortenedUrl>()))
            .Callback<UpdateShortenedUrlCommand, ShortenedUrl>((src, dest) =>
            {
                dest.OriginalUrl = src.OriginalUrl;
                dest.ShortCode = src.ShortCode;
            });
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

        var sut = new UpdateShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object, _mapperMock.Object);
        var result = await sut.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUniqueId(TestUniqueId, CancellationToken.None), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateCachedUrlRecordNotification>(), CancellationToken.None), Times.Once);

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
    public void Handles_NonExistingUniqueId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.ShortCodeExistsAsync(TestShortCode2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateShortenedUrlCommand
        {
            UniqueId = TestUniqueId,
            OriginalUrl = TestUrl,
            ShortCode = TestShortCode2
        };

        var sut = new UpdateShortenedUrlCommandHandler(_repositoryMock.Object, _mediatorMock.Object, _mapperMock.Object);
        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(request, CancellationToken.None));
        Assert.That(exception.Message, Is.EqualTo("Selected custom code is taken. Choose different one."));

        _repositoryMock.Verify(r => r.GetByUniqueId(TestUniqueId, CancellationToken.None), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Never);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateCachedUrlRecordNotification>(), CancellationToken.None), Times.Never);
    }
}