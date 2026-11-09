using StackExchange.Redis;
using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Infrastructure.Redis;

public class RedisTokenRevocationStore : ITokenRevocationStore
{
    private readonly IConnectionMultiplexer _redis;
    private const string KeyPrefix = "revoked_token:";

    public RedisTokenRevocationStore(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private IDatabase Db => _redis.GetDatabase();

    public async Task RevokeTokenAsync(string token, TimeSpan ttl, string? reason = null, CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{token}";
        var val = reason ?? "revoked";
        await Db.StringSetAsync(key, val, ttl);
    }

    public async Task RevokeTokenUntilAsync(string token, DateTime expiryUtc, string? reason = null, CancellationToken cancellationToken = default)
    {
        var ttl = expiryUtc - DateTime.UtcNow;
        if (ttl > TimeSpan.Zero)
        {
            await RevokeTokenAsync(token, ttl, reason, cancellationToken);
        }
    }

    public async Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{token}";
        return await Db.KeyExistsAsync(key);
    }

    public async Task<CheckTokenResponseDto> GetRevocationDetailsAsync(string token, CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{token}";
        var val = await Db.StringGetAsync(key);

        return new CheckTokenResponseDto
        {
            Token = token,
            IsRevoked = val.HasValue,
            Reason = val.HasValue ? val.ToString() : null
        };
    }

    public Task<int> PurgeExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        // Redis natively evicts keys via TTL expiration.
        return Task.FromResult(0);
    }
}
