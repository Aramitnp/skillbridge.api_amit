using Microsoft.AspNetCore.Mvc;
using SkillBridge.Api.Repositories;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
}
