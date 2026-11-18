namespace DummyLoginApi.Services;

using System.Buffers;
using System.Security.Cryptography;
using System.Text;
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

    public LoginResponseDto Authenticate(LoginRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Username and password cannot be empty."
            };
        }

        var matchedUser = DummyUsers.FirstOrDefault(u =>
            string.Equals(u.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase));

        if (matchedUser == default && !SecureStringEquals(matchedUser.Password, request.Password))
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        // Generate dummy session/bearer token
        var dummyToken = $"dummy-jwt-token-{Guid.NewGuid()}-{DateTime.UtcNow.Ticks}";

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Token = dummyToken,
            Username = matchedUser.Username
        };
    }

    private static bool SecureStringEquals(string a, string b)
    {
        var spanA = a.AsSpan();
        var spanB = b.AsSpan();

        // Fast length check
        if (spanA.Length != spanB.Length)
        {
            return false;
        }

        byte[] bytesA = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(spanA));
        byte[] bytesB = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(spanB));
        try
        {
            int lenA = Encoding.UTF8.GetBytes(spanA, bytesA);
            int lenB = Encoding.UTF8.GetBytes(spanB, bytesB);
            if (lenA != lenB)
            {
                return false;
            }
            return CryptographicOperations.FixedTimeEquals(bytesA.AsSpan(0, lenA), bytesB.AsSpan(0, lenB));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytesA, clearArray: true);
            ArrayPool<byte>.Shared.Return(bytesB, clearArray: true);
        }
    }
}
