using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Application.Services
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly ITokenRevocationStore _store;
        private readonly IConfiguration _configuration;

        public TokenRevocationService(
            ITokenRevocationStore store,
            IConfiguration configuration)
        {
            _store = store;
            _configuration = configuration;
        }

        public async Task RevokeAsync(
            RevokeTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Jti))
                throw new ArgumentException("Jti is required.");

            var defaultTtlMinutes = int.Parse(
                _configuration["TokenRevocation:DefaultTtlMinutes"] ?? "20");

            var ttl = request.TtlSeconds.HasValue
                ? TimeSpan.FromSeconds(request.TtlSeconds.Value)
                : TimeSpan.FromMinutes(defaultTtlMinutes);

            // Always revoke the specific token
            await _store.RevokeAsync(request.Jti, ttl, cancellationToken);

            // Optionally revoke all tokens for the user ("logout everywhere")
            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                await _store.RevokeUserAsync(request.UserId, ttl, cancellationToken);
            }
        }

        public async Task<CheckTokenResponseDto> CheckAsync(
            CheckTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Jti))
                throw new ArgumentException("Jti is required.");

            // Check specific token first
            if (await _store.IsRevokedAsync(request.Jti, cancellationToken))
            {
                return new CheckTokenResponseDto
                {
                    IsRevoked = true,
                    Reason = "Token has been revoked"
                };
            }

            // Check if the entire user session was revoked
            if (!string.IsNullOrWhiteSpace(request.UserId) &&
                await _store.IsUserRevokedAsync(request.UserId, cancellationToken))
            {
                return new CheckTokenResponseDto
                {
                    IsRevoked = true,
                    Reason = "All tokens for this user have been revoked"
                };
            }

            return new CheckTokenResponseDto
            {
                IsRevoked = false
            };
        }
    }

}
