using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Create(CreateUserRequestDto request)
    {
        var message = await _userRepository.CreateUserAsync(request);
        return Ok(new { message });
    }
}
