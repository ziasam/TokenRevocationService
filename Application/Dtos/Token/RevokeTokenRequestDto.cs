namespace TokenRevocationService.Application.Dtos.Token;

public class RevokeTokenRequestDto
{
    public string Token { get; set; } = string.Empty;
    public TimeSpan? Expiry { get; set; }
    public long? ExpiresInSeconds { get; set; }
    public DateTime? ExpiryUtc { get; set; }
    public string? Reason { get; set; }
}
