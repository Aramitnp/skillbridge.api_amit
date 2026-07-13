using Microsoft.AspNetCore.Mvc;
using SkillBridge.Api.Repositories;
using SkillBridge.Api.Repositories.Interfaces;
using SkillBridge.Api.Services.Interfaces;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserController(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<UserRegistrationResponseDto>> Create(
        CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.CreateUserAsync(request, cancellationToken);
            var response = new UserRegistrationResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Type = user.Type,
                CreatedAt = user.CreatedAt
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (DuplicateEmailException)
        {
            return Conflict(new
            {
                message = "A user with this email address already exists."
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var loginResult = await _userRepository.AuthenticateAsync(
            request,
            cancellationToken);

        if (loginResult.Status == UserLoginStatus.InvalidCredentials)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        if (loginResult.Status == UserLoginStatus.Inactive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "This account is inactive."
            });
        }

        var token = _tokenService.CreateAccessToken(loginResult);

        return Ok(new LoginResponseDto
        {
            AccessToken = token.Value,
            TokenType = "Bearer",
            ExpiresAt = token.ExpiresAt,
            UserId = loginResult.UserId,
            Name = loginResult.Name,
            Email = loginResult.Email,
            Type = loginResult.Type
        });
    }
}
