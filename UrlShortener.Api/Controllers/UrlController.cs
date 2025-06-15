using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Features.Urls.Queries;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("/shorten")]
    public class UrlController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UrlController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Url))
                {
                    return BadRequest("URL address to shorten cannot be empty.");
                }

                var result = await _mediator.Send(new ShortenUrlCommand
                {
                    Url = request.Url,
                    CustomCode = request.CustomCode
                });

                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectUrl(string code)
        {
            if(string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Short code cannot be empty.");
            }

            var query = new GetUrlByCodeQuery { ShortCode = code };
            var originalUrl = await _mediator.Send(query);

            if (originalUrl == null)
            {
                return NotFound("No URL is associated with this code.");
            }

            return Redirect(originalUrl);
        }
    }
}
