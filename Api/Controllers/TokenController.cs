using Microsoft.AspNetCore.Mvc;
using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Api.Controllers
{
    [ApiController]
    [Route("api/tokens")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenRevocationService _service;

        public TokenController(ITokenRevocationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Revoke a specific access token (called by Authentication Service on logout)
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(
            [FromBody] RevokeTokenRequestDto request,
            CancellationToken cancellationToken)
        {
            await _service.RevokeAsync(request, cancellationToken);
            return Ok(new { message = "Token revoked successfully." });
        }

        /// <summary>
        /// Check if a token is revoked (called by all microservices)
        /// </summary>
        [HttpPost("check")]
        public async Task<ActionResult<CheckTokenResponseDto>> Check(
            [FromBody] CheckTokenRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CheckAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}