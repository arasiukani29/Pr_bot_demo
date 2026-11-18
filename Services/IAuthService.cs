namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IAuthService
{
    // Ensure cancellation token is passed if using async operations
    Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
