namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public class AuthService : IAuthService
{
    // In-memory dummy users storage (thread-safe read-only collection)
    private static readonly IReadOnlyCollection<(string Username, string Password)> DummyUsers = new List<(string Username, string Password)>
    {
        ("admin", "admin123"),
        ("user", "password123"),
        ("demo", "demo@123")
    }.AsReadOnly();

    public Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(new LoginResponseDto
            {
                Success = false,
                Message = "Username and password cannot be empty."
            });
        }

        var matchedUser = DummyUsers.FirstOrDefault(u =>
            string.Equals(u.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase));

        if (matchedUser == default)
        {
            return Task.FromResult(new LoginResponseDto
            {
                Success = false,
                Message = "User not found."
            });
        }

        if (matchedUser.Password != request.Password)
        {
            return Task.FromResult(new LoginResponseDto
            {
                Success = false,
                Message = "Invalid password."
            });
        }

        // Generate dummy session/bearer token
        var dummyToken = $"dummy-jwt-token-{Guid.NewGuid()}-{DateTime.UtcNow.Ticks}";

        return Task.FromResult(new LoginResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = dummyToken,
            Username = matchedUser.Username
        });
    }
}
