using Microsoft.AspNetCore.Mvc;
using SkillBridge.Web.Filters;
using SkillBridge.Web.Services;
using SkillBridge.Web.ViewModels;

namespace SkillBridge.Web.Controllers;

[RoleRequired("Applicant")]
public class ApplicantController : Controller
{
    private readonly ApiService _api;

    public ApplicantController(ApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<ApplicantDashboardViewModel>(
            "api/dashboard/applicant",
            cancellationToken);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        return View(result.Data ?? new ApplicantDashboardViewModel());
    }

    public async Task<IActionResult> Applications(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<List<JobApplicationViewModel>>(
            "api/job-applications/my-applications",
            cancellationToken);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        return View(result.Data ?? []);
    }
}
