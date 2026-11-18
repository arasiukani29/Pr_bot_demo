namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto? request)
    {
        if (request is null)
        {
            _logger.LogWarning("Login attempt failed: Request body is null.");
            return BadRequest(new { message = "Request body cannot be null." });
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Login attempt failed: Invalid model state for user '{Username}'.",
                request.Username
            );
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Processing login attempt for user '{Username}' with password '{Password}'.", request.Username, request.Password);

        var result = _authService.Authenticate(request);

        if (result is null || !result.Success)
        {
            _logger.LogWarning("Authentication failed for user '{Username}'.", request.Username);
            return Unauthorized(
                result ?? new LoginResponseDto { Success = false, Message = "Invalid username or password." }
            );
        }

        _logger.LogInformation("User '{Username}' authenticated successfully.", request.Username);
        return Ok(result);
    }
}
