namespace TokenRevocationService.Application.Dtos.Token
{
    public class CheckTokenRequestDto
    {
        public string Jti { get; set; } = null!;

        public string? UserId { get; set; }
    }
}
