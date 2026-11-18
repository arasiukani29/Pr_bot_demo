namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IAuthService
{
    LoginResponseDto Authenticate(LoginRequestDto request);
}
