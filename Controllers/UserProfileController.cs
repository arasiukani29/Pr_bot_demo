namespace DummyLoginApi.Controllers;

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

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var destinationPath = Path.Combine(uploadFolder, file.FileName);

        using (var stream = new FileStream(destinationPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var avatarUrl = $"/uploads/avatars/{file.FileName}";

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
