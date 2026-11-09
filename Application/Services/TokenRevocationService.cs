using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Application.Services;

public class TokenRevocationService : ITokenRevocationService
{
    private readonly ITokenRevocationStore _store;
    private readonly ILogger<TokenRevocationService> _logger;

    public TokenRevocationService(ITokenRevocationStore store, ILogger<TokenRevocationService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public async Task RevokeAsync(RevokeTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(request.Token));
        }

        if (request.ExpiryUtc.HasValue)
        {
            await _store.RevokeTokenUntilAsync(request.Token, request.ExpiryUtc.Value, request.Reason, cancellationToken);
            return;
        }

        var ttl = request.Expiry 
                  ?? (request.ExpiresInSeconds.HasValue ? TimeSpan.FromSeconds(request.ExpiresInSeconds.Value) : TimeSpan.FromDays(1));

        await _store.RevokeTokenAsync(request.Token, ttl, request.Reason, cancellationToken);
        _logger.LogInformation("Token {Token} revoked with TTL {Ttl}", request.Token, ttl);
    }

    public async Task<CheckTokenResponseDto> CheckAsync(CheckTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return new CheckTokenResponseDto { Token = string.Empty, IsRevoked = false };
        }

        return await _store.GetRevocationDetailsAsync(request.Token, cancellationToken);
    }

    public async Task<bool> IsRevokedAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return await _store.IsTokenRevokedAsync(token, cancellationToken);
    }

    public async Task<int> PurgeExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await _store.PurgeExpiredTokensAsync(cancellationToken);
    }
}
