using StackExchange.Redis;
using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Infrastructure.Redis
{
    public class RedisTokenRevocationStore : ITokenRevocationStore
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        private const string JtiPrefix = "revoked:jti:";
        private const string UserPrefix = "revoked:user:";

        public RedisTokenRevocationStore(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        public async Task RevokeAsync(
            string jti,
            TimeSpan ttl,
            CancellationToken cancellationToken = default)
        {
            var key = JtiPrefix + jti;
            await _db.StringSetAsync(key, "1", ttl);
        }

        public async Task<bool> IsRevokedAsync(
            string jti,
            CancellationToken cancellationToken = default)
        {
            var key = JtiPrefix + jti;
            return await _db.KeyExistsAsync(key);
        }

        public async Task RevokeUserAsync(
            string userId,
            TimeSpan ttl,
            CancellationToken cancellationToken = default)
        {
            var key = UserPrefix + userId;
            await _db.StringSetAsync(key, "1", ttl);
        }

        public async Task<bool> IsUserRevokedAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var key = UserPrefix + userId;
            return await _db.KeyExistsAsync(key);
        }
    }
}