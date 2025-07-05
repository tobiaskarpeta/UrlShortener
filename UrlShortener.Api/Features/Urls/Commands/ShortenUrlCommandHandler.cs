using MediatR;
using UrlShortener.Api.Models;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Features.Urls.Commands
{
    public class ShortenUrlCommandHandler : IRequestHandler<ShortenUrlCommand, ShortenedUrl>
    {
        private readonly IUrlRepository _repository;

        public ShortenUrlCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<ShortenedUrl> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            var shortCode = request.CustomCode ?? ShortCodeGenerator.Generate();

            while (await _repository.ShortCodeExistsAsync(shortCode, cancellationToken))
            {
                if (string.Equals(request.CustomCode, shortCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new InvalidOperationException("Custom short code is already taken. Please choose a different one.");
                }

                shortCode = ShortCodeGenerator.Generate();
            }

            var shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = request.Url,
                ShortCode = shortCode,
                CreatedAt = DateTime.Now,
            };

            await _repository.AddAsync(shortenedUrl, cancellationToken);
            return shortenedUrl;
        }
    }
}
