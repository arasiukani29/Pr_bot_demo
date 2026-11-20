namespace DummyLoginApi.Controllers;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DummyLoginApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private static readonly Dictionary<string, UserProfileDto> Profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = new UserProfileDto
        {
            Username = "admin",
            DisplayName = "System Administrator",
            Email = "admin@example.com",
            Bio = "Managing core system configurations and user access controls.",
            AvatarUrl = "/avatars/default-admin.png"
        },
        ["user"] = new UserProfileDto
        {
            Username = "user",
            DisplayName = "Standard User",
            Email = "user@example.com",
            Bio = "Regular application user.",
            AvatarUrl = "/avatars/default-user.png"
        }
    };

    private const long MaxAvatarFileSize = 5 * 1024 * 1024; // 5 MB

    private static readonly HashSet<string> AllowedAvatarExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(ILogger<UserProfileController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{username}")]
    public IActionResult GetProfile(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new { message = "Username cannot be empty." });
        }

        if (Profiles.TryGetValue(username.Trim(), out var profile))
        {
            return Ok(profile);
        }

        return NotFound(new { message = $"Profile for '{username}' was not found." });
    }

    [Authorize]
    [HttpPut]
    public IActionResult UpdateProfile([FromBody] UserProfileDto updatedProfile)
    {
        if (updatedProfile is null || string.IsNullOrWhiteSpace(updatedProfile.Username))
        {
            return BadRequest(new { message = "Invalid profile payload." });
        }

        var authenticatedUser = GetAuthenticatedUsername();
        if (string.IsNullOrWhiteSpace(authenticatedUser))
        {
            return Unauthorized(new { message = "Authentication token or identity is required." });
        }

        if (!string.Equals(authenticatedUser, updatedProfile.Username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(authenticatedUser, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to modify this profile." });
        }

        if (updatedProfile.Username.Length > 50 ||
            (updatedProfile.DisplayName?.Length ?? 0) > 100 ||
            (updatedProfile.Email?.Length ?? 0) > 150 ||
            (updatedProfile.Bio?.Length ?? 0) > 500 ||
            (updatedProfile.AvatarUrl?.Length ?? 0) > 2048)
        {
            return BadRequest(new { message = "Profile fields exceed maximum allowed character limits." });
        }

        if (string.IsNullOrWhiteSpace(updatedProfile.Email) ||
            !new EmailAddressAttribute().IsValid(updatedProfile.Email.Trim()))
        {
            return BadRequest(new { message = "A valid email format is required." });
        }

        var key = updatedProfile.Username.Trim();
        Profiles[key] = updatedProfile;

        return Ok(new { message = "Profile updated successfully.", profile = updatedProfile });
    }

    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile? file, [FromQuery] string? username)
    {
        var authenticatedUser = GetAuthenticatedUsername();
        if (string.IsNullOrWhiteSpace(authenticatedUser))
        {
            return Unauthorized(new AvatarUploadResponseDto
            {
                Success = false,
                Message = "Authentication is required to upload an avatar."
            });
        }

        var targetUsername = string.IsNullOrWhiteSpace(username) ? authenticatedUser : username.Trim();

        if (!string.Equals(authenticatedUser, targetUsername, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(authenticatedUser, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new AvatarUploadResponseDto
            {
                Success = false,
                Message = "You do not have permission to update another user's avatar."
            });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new AvatarUploadResponseDto
            {
                Success = false,
                Message = "Please select a file to upload."
            });
        }

        if (file.Length > MaxAvatarFileSize)
        {
            return BadRequest(new AvatarUploadResponseDto
            {
                Success = false,
                Message = "File size exceeds the maximum allowed limit of 5 MB."
            });
        }

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedAvatarExtensions.Contains(extension))
        {
            return BadRequest(new AvatarUploadResponseDto
            {
                Success = false,
                Message = "Invalid file type. Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed."
            });
        }

        var safeFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant();
        var destinationPath = Path.Combine(uploadFolder, safeFileName);

        using (var stream = new FileStream(destinationPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var avatarUrl = $"/uploads/avatars/{safeFileName}";

        if (Profiles.TryGetValue(targetUsername, out var profile))
        {
            profile.AvatarUrl = avatarUrl;
        }

        return Ok(new AvatarUploadResponseDto
        {
            Success = true,
            Message = "Avatar image uploaded successfully.",
            AvatarUrl = avatarUrl
        });
    }

    private string? GetAuthenticatedUsername()
    {
        var identityName = User.Identity?.Name ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(identityName))
        {
            return identityName;
        }

        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (token.StartsWith("jwt-dummy-token-", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var base64Part = token.Substring("jwt-dummy-token-".Length);
                    return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Part));
                }
                catch
                {
                    // Ignore decoding error
                }
            }
            return token;
        }

        return null;
    }
}
