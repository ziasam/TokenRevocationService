namespace TokenRevocationService.Application.Dtos.Token
{
    public class RevokeTokenRequestDto
    {
        /// <summary>
        /// JWT ID (jti claim)
        /// </summary>
        public string Jti { get; set; } = null!;

        /// <summary>
        /// Optional: User ID (sub claim) – useful for "logout everywhere"
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Remaining lifetime of the token in seconds.
        /// If not provided, a default TTL is used.
        /// </summary>
        public int? TtlSeconds { get; set; }
    }
}
