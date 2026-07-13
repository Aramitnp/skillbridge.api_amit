using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Api.Repositories.Interfaces;

namespace SkillBridge.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IJobApplicationRepository _applicationRepository;

    public DashboardController(IJobApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    [Authorize(Policy = SupportedUserTypes.Applicant)]
    [HttpGet("applicant")]
    public async Task<ActionResult<ApplicantDashboardDto>> GetApplicantDashboard(
        CancellationToken cancellationToken)
    {
        return TryGetUserId(out var applicantId)
            ? Ok(await _applicationRepository.GetApplicantDashboardAsync(
                applicantId,
                cancellationToken))
            : Unauthorized();
    }

    [Authorize(Policy = SupportedUserTypes.Company)]
    [HttpGet("company")]
    public async Task<ActionResult<CompanyDashboardDto>> GetCompanyDashboard(
        CancellationToken cancellationToken)
    {
        return TryGetUserId(out var companyId)
            ? Ok(await _applicationRepository.GetCompanyDashboardAsync(
                companyId,
                cancellationToken))
            : Unauthorized();
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            out userId);
    }
}
