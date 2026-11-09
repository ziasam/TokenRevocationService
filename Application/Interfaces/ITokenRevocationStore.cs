namespace TokenRevocationService.Application.Interfaces
{
    public interface ITokenRevocationStore
    {
        Task RevokeAsync(string jti, TimeSpan ttl, CancellationToken cancellationToken = default);

        Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default);

        Task RevokeUserAsync(string userId, TimeSpan ttl, CancellationToken cancellationToken = default);

        Task<bool> IsUserRevokedAsync(string userId, CancellationToken cancellationToken = default);
    }
}
