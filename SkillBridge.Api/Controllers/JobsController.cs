using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _jobRepository;

    public JobsController(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PaginatedJobResponseDto>> GetJobs(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? location = null,
        string? jobType = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize is < 1 or > 100)
        {
            return BadRequest(new
            {
                message = "Page number must be at least 1 and page size must be between 1 and 100."
            });
        }

        return Ok(await _jobRepository.GetActiveJobsAsync(
            pageNumber,
            pageSize,
            search,
            location,
            jobType,
            cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobResponseDto>> GetJob(
        int id,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetJobByIdAsync(id, cancellationToken);
        return job is null ? NotFound() : Ok(job);
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpGet("my-jobs")]
    public async Task<ActionResult<IReadOnlyList<JobResponseDto>>> GetMyJobs(
        CancellationToken cancellationToken)
    {
        return TryGetUserId(out var companyId)
            ? Ok(await _jobRepository.GetCompanyJobsAsync(companyId, cancellationToken))
            : Unauthorized();
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpPost]
    public async Task<ActionResult<JobResponseDto>> CreateJob(
        CreateJobRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var companyId))
        {
            return Unauthorized();
        }

        var job = await _jobRepository.CreateJobAsync(
            companyId,
            request,
            cancellationToken);

        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<JobResponseDto>> UpdateJob(
        int id,
        UpdateJobRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var companyId))
        {
            return Unauthorized();
        }

        var ownerId = await _jobRepository.GetJobCompanyIdAsync(id, cancellationToken);
        if (!ownerId.HasValue)
        {
            return NotFound();
        }

        if (ownerId.Value != companyId)
        {
            return Forbid();
        }

        var job = await _jobRepository.UpdateJobAsync(
            id,
            companyId,
            request,
            cancellationToken);

        return job is null ? NotFound() : Ok(job);
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateJob(
        int id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var companyId))
        {
            return Unauthorized();
        }

        var ownerId = await _jobRepository.GetJobCompanyIdAsync(id, cancellationToken);
        if (!ownerId.HasValue)
        {
            return NotFound();
        }

        if (ownerId.Value != companyId)
        {
            return Forbid();
        }

        return await _jobRepository.DeactivateJobAsync(
            id,
            companyId,
            cancellationToken)
            ? NoContent()
            : NotFound();
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            out userId);
    }
}
