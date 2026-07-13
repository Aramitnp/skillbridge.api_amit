using Microsoft.AspNetCore.Mvc;
using SkillBridge.Web.Filters;
using SkillBridge.Web.Services;
using SkillBridge.Web.ViewModels;

namespace SkillBridge.Web.Controllers;

public class JobsController : Controller
{
    private readonly ApiService _api;

    public JobsController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        string? location,
        string? jobType,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        var query = $"api/jobs?pageNumber={page}&pageSize=3" +
            $"&search={Uri.EscapeDataString(search ?? string.Empty)}" +
            $"&location={Uri.EscapeDataString(location ?? string.Empty)}" +
            $"&jobType={Uri.EscapeDataString(jobType ?? string.Empty)}";
        var result = await _api.GetAsync<PaginatedJobResponse>(query, cancellationToken);

        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        var response = result.Data ?? new PaginatedJobResponse { CurrentPage = page };
        return View(new JobListViewModel
        {
            Jobs = response.Items,
            Search = search,
            Location = location,
            JobType = jobType,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages,
            TotalRecords = response.TotalRecords
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<JobViewModel>($"api/jobs/{id}", cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Job not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new JobDetailsViewModel
        {
            Job = result.Data,
            IsLoggedIn = !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionKeys.AccessToken)),
            UserType = HttpContext.Session.GetString(SessionKeys.UserType)
        });
    }

    [RoleRequired("Applicant")]
    [HttpGet]
    public async Task<IActionResult> Apply(int id, CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<JobViewModel>($"api/jobs/{id}", cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Job not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new ApplyJobViewModel { JobId = id, JobTitle = result.Data.Title });
    }

    [RoleRequired("Applicant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyJobViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.PostAsync<JobApplicationViewModel>("api/job-applications", new
        {
            model.JobId,
            model.CoverLetter
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Application could not be submitted.");
            return View(model);
        }

        TempData["Success"] = "Your application was submitted.";
        return RedirectToAction("Applications", "Applicant");
    }
}
