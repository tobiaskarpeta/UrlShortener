using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Features.Urls.Queries;

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

        [HttpPost("create")]
        public async Task<IActionResult> ShortenUrl([FromBody] CreateShortenUrlRequest request)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

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

        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetShortenedUrl(string id)
        {
            try
            {
                var result = await _mediator.Send(new GetShortenedUrlQuery(id));
                return result != null ? Ok(result) : NotFound();
            }
            catch
            {
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateShortenedUrl(string id, [FromBody] UpdateShortenedUrlRequest request)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(new UpdateShortenedUrlCommand
                {
                    UniqueId = id,
                    OriginalUrl = request.OriginalUrl,
                    ShortCode = request.ShortCode,
                    ExpiresAt = request.ExpiresAt
                });

                return result != null ? Ok(result) : NotFound();
            }
            catch
            {
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }

        [HttpDelete("delete/{id}")]
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
