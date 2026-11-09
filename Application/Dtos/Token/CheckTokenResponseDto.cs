namespace TokenRevocationService.Application.Dtos.Token;

public class CheckTokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? Reason { get; set; }
}
