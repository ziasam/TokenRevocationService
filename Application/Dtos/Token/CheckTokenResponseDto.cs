namespace TokenRevocationService.Application.Dtos.Token
{
    public class CheckTokenResponseDto
    {
        public bool IsRevoked { get; set; }

        public string? Reason { get; set; }
    }
}
