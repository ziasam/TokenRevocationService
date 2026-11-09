using TokenRevocationService.Application.Dtos.Token;

namespace TokenRevocationService.Application.Interfaces
{
    public interface ITokenRevocationService
    {
        Task RevokeAsync(RevokeTokenRequestDto request, CancellationToken cancellationToken = default);

        Task<CheckTokenResponseDto> CheckAsync(CheckTokenRequestDto request, CancellationToken cancellationToken = default);
    }
}
