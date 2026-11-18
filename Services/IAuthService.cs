namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IAuthService
{
    // Ensure nullable reference validation or null-guard
    LoginResponseDto Authenticate(LoginRequestDto request);
}
