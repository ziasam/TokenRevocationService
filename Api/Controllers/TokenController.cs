using Microsoft.AspNetCore.Mvc;
using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenRevocationService _revocationService;

    public TokenController(ITokenRevocationService revocationService)
    {
        _revocationService = revocationService;
    }

    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequestDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(new { message = "Token is required." });
        }

        await _revocationService.RevokeAsync(request, ct);
        return Ok(new { message = "Token revoked successfully.", token = request.Token });
    }

    [HttpPost("check")]
    [ProducesResponseType(typeof(CheckTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckTokenResponseDto>> Check([FromBody] CheckTokenRequestDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(new { message = "Token is required." });
        }

        var result = await _revocationService.CheckAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("is-revoked/{token}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> IsRevoked([FromRoute] string token, CancellationToken ct)
    {
        var isRevoked = await _revocationService.IsRevokedAsync(token, ct);
        return Ok(isRevoked);
    }

    [HttpPost("purge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PurgeExpired(CancellationToken ct)
    {
        var count = await _revocationService.PurgeExpiredAsync(ct);
        return Ok(new { purgedCount = count });
    }
}
