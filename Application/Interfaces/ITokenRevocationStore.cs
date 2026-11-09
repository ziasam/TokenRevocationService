using TokenRevocationService.Application.Dtos.Token;

namespace TokenRevocationService.Application.Interfaces;

public interface ITokenRevocationStore
{
    Task RevokeTokenAsync(string token, TimeSpan ttl, string? reason = null, CancellationToken cancellationToken = default);
    Task RevokeTokenUntilAsync(string token, DateTime expiryUtc, string? reason = null, CancellationToken cancellationToken = default);
    Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default);
    Task<CheckTokenResponseDto> GetRevocationDetailsAsync(string token, CancellationToken cancellationToken = default);
    Task<int> PurgeExpiredTokensAsync(CancellationToken cancellationToken = default);
}
