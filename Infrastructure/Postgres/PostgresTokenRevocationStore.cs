using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TokenRevocationService.Application.Dtos.Token;
using TokenRevocationService.Application.Interfaces;
using TokenRevocationService.Domain.Entities;

namespace TokenRevocationService.Infrastructure.Postgres;

public class PostgresTokenRevocationStore : ITokenRevocationStore
{
    private readonly TokenRevocationDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PostgresTokenRevocationStore> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    public PostgresTokenRevocationStore(
        TokenRevocationDbContext context,
        IMemoryCache memoryCache,
        ILogger<PostgresTokenRevocationStore> logger)
    {
        _context = context;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task RevokeTokenAsync(string token, TimeSpan ttl, string? reason = null, CancellationToken cancellationToken = default)
    {
        var expiryUtc = DateTime.UtcNow.Add(ttl);
        await RevokeTokenUntilAsync(token, expiryUtc, reason, cancellationToken);
    }

    public async Task RevokeTokenUntilAsync(string token, DateTime expiryUtc, string? reason = null, CancellationToken cancellationToken = default)
    {
        var existing = await _context.RevokedTokens.FindAsync(new object[] { token }, cancellationToken);

        if (existing != null)
        {
            existing.ExpiresAtUtc = expiryUtc;
            existing.Reason = reason ?? existing.Reason;
            existing.RevokedAtUtc = DateTime.UtcNow;
        }
        else
        {
            var record = new RevokedTokenRecord
            {
                Token = token,
                RevokedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = expiryUtc,
                Reason = reason
            };
            await _context.RevokedTokens.AddAsync(record, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _memoryCache.Set($"revoked_{token}", true, CacheDuration);
    }

    public async Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"revoked_{token}";
        if (_memoryCache.TryGetValue(cacheKey, out bool isRevoked) && isRevoked)
        {
            return true;
        }

        var revoked = await _context.RevokedTokens
            .AsNoTracking()
            .AnyAsync(r => r.Token == token && r.ExpiresAtUtc > DateTime.UtcNow, cancellationToken);

        if (revoked)
        {
            _memoryCache.Set(cacheKey, true, CacheDuration);
        }

        return revoked;
    }

    public async Task<CheckTokenResponseDto> GetRevocationDetailsAsync(string token, CancellationToken cancellationToken = default)
    {
        var record = await _context.RevokedTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

        if (record == null || record.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return new CheckTokenResponseDto
            {
                Token = token,
                IsRevoked = false
            };
        }

        return new CheckTokenResponseDto
        {
            Token = record.Token,
            IsRevoked = true,
            RevokedAtUtc = record.RevokedAtUtc,
            Reason = record.Reason
        };
    }

    public async Task<int> PurgeExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var deletedCount = await _context.RevokedTokens
            .Where(r => r.ExpiresAtUtc <= now)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation("Purged {Count} expired token revocation records from PostgreSQL", deletedCount);
        }

        return deletedCount;
    }
}
