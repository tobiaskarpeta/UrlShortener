using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Features.Urls.Commands;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/shorten")]
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
            catch
            {
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShortenedUrl(string id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteShortenedUrlCommand(id));
                return result ? NoContent() : NotFound();
            }
            catch
            {
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }
    }
}
