namespace DummyLoginApi.Controllers;

using System.ComponentModel.DataAnnotations;
using DummyLoginApi.Models;
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

    [HttpPut]
    public IActionResult UpdateProfile([FromBody] UserProfileDto updatedProfile)
    {
        if (updatedProfile is null || string.IsNullOrWhiteSpace(updatedProfile.Username))
        {
            return BadRequest(new { message = "Invalid profile payload." });
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

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile? file, [FromQuery] string username)
    {
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

        if (!string.IsNullOrWhiteSpace(username) && Profiles.TryGetValue(username.Trim(), out var profile))
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
}
