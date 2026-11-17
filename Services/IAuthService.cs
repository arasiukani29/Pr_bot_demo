namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IAuthService
{
    Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request);
}
