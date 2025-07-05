using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Features.Urls.Queries;

namespace UrlShortener.Api.Controllers;

[ApiController]
public class RedirectionController : ControllerBase
{
    private readonly IMediator _mediator;

    public RedirectionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectUrl(string code)
    {
        var query = new GetUrlByCodeQuery(code);
        var originalUrl = await _mediator.Send(query);

        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            return NotFound("No URL is associated with this code.");
        }

        return Redirect(originalUrl);
    }
}