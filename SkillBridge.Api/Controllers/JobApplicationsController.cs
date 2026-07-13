using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Api.Repositories;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("api/job-applications")]
public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;

    public JobApplicationsController(
        IJobApplicationRepository applicationRepository,
        IJobRepository jobRepository)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
    }

    [Authorize(Policy = SupportedUserTypes.Applicant)]
    [HttpPost]
    public async Task<ActionResult<JobApplicationResponseDto>> Apply(
        CreateJobApplicationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var applicantId))
        {
            return Unauthorized();
        }

        try
        {
            var application = await _applicationRepository.ApplyAsync(
                applicantId,
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, application);
        }
        catch (JobNotFoundException)
        {
            return NotFound(new { message = "Job was not found." });
        }
        catch (JobUnavailableException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (DuplicateJobApplicationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [Authorize(Policy = SupportedUserTypes.Applicant)]
    [HttpGet("my-applications")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationResponseDto>>> GetMine(
        CancellationToken cancellationToken)
    {
        return TryGetUserId(out var applicantId)
            ? Ok(await _applicationRepository.GetApplicantApplicationsAsync(
                applicantId,
                cancellationToken))
            : Unauthorized();
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpGet("received")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationResponseDto>>> GetReceived(
        CancellationToken cancellationToken)
    {
        return TryGetUserId(out var companyId)
            ? Ok(await _applicationRepository.GetReceivedApplicationsAsync(
                companyId,
                cancellationToken: cancellationToken))
            : Unauthorized();
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpGet("job/{jobId:int}")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationResponseDto>>> GetForJob(
        int jobId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var companyId))
        {
            return Unauthorized();
        }

        var ownerId = await _jobRepository.GetJobCompanyIdAsync(
            jobId,
            cancellationToken);
        if (!ownerId.HasValue)
        {
            return NotFound();
        }

        if (ownerId.Value != companyId)
        {
            return Forbid();
        }

        return Ok(await _applicationRepository.GetReceivedApplicationsAsync(
            companyId,
            jobId,
            cancellationToken));
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<JobApplicationResponseDto>> UpdateStatus(
        int id,
        UpdateApplicationStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var companyId))
        {
            return Unauthorized();
        }

        var ownerId = await _applicationRepository.GetApplicationCompanyIdAsync(
            id,
            cancellationToken);
        if (!ownerId.HasValue)
        {
            return NotFound();
        }

        if (ownerId.Value != companyId)
        {
            return Forbid();
        }

        var application = await _applicationRepository.UpdateStatusAsync(
            id,
            companyId,
            request.Status,
            cancellationToken);

        return application is null ? NotFound() : Ok(application);
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            out userId);
    }
}
