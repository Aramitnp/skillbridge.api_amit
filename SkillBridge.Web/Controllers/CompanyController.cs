using Microsoft.AspNetCore.Mvc;
using SkillBridge.Web.Filters;
using SkillBridge.Web.Services;
using SkillBridge.Web.ViewModels;

namespace SkillBridge.Web.Controllers;

[RoleRequired("Company")]
public class CompanyController : Controller
{
    private readonly ApiService _api;

    public CompanyController(ApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<CompanyDashboardViewModel>(
            "api/dashboard/company",
            cancellationToken);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        return View(result.Data ?? new CompanyDashboardViewModel());
    }

    public async Task<IActionResult> MyJobs(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<List<JobViewModel>>("api/jobs/my-jobs", cancellationToken);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        return View(result.Data ?? []);
    }

    [HttpGet]
    public IActionResult Create() => View(new JobFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.PostAsync<JobViewModel>("api/jobs", ToRequest(model), cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Job could not be created.");
            return View(model);
        }

        TempData["Success"] = "Job created successfully.";
        return RedirectToAction(nameof(MyJobs));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<List<JobViewModel>>("api/jobs/my-jobs", cancellationToken);
        var job = result.Data?.SingleOrDefault(item => item.Id == id);
        if (!result.IsSuccess || job is null)
        {
            TempData["Error"] = result.Error ?? "Job not found.";
            return RedirectToAction(nameof(MyJobs));
        }

        return View(new EditJobViewModel
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            MinimumSalary = job.MinimumSalary,
            MaximumSalary = job.MaximumSalary,
            Location = job.Location,
            JobType = job.JobType,
            Deadline = job.Deadline.ToLocalTime()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditJobViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.PutAsync<JobViewModel>(
            $"api/jobs/{model.Id}",
            ToRequest(model),
            cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Job could not be updated.");
            return View(model);
        }

        TempData["Success"] = "Job updated successfully.";
        return RedirectToAction(nameof(MyJobs));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await _api.DeleteAsync($"api/jobs/{id}", cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Job deactivated."
            : result.Error ?? "Job could not be deactivated.";
        return RedirectToAction(nameof(MyJobs));
    }

    public async Task<IActionResult> Applications(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<List<JobApplicationViewModel>>(
            "api/job-applications/received",
            cancellationToken);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.Error;
        }

        return View(result.Data ?? []);
    }

    public async Task<IActionResult> JobApplications(int id, CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<List<JobApplicationViewModel>>(
            $"api/job-applications/job/{id}",
            cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Applications could not be loaded.";
            return RedirectToAction(nameof(MyJobs));
        }

        ViewData["JobId"] = id;
        return View(result.Data ?? []);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id,
        int? jobId,
        string status,
        CancellationToken cancellationToken)
    {
        var result = await _api.PutAsync<JobApplicationViewModel>(
            $"api/job-applications/{id}/status",
            new { status },
            cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Application status updated."
            : result.Error ?? "Status could not be updated.";

        return jobId.HasValue
            ? RedirectToAction(nameof(JobApplications), new { id = jobId.Value })
            : RedirectToAction(nameof(Applications));
    }

    private static object ToRequest(JobFormViewModel model) => new
    {
        model.Title,
        model.Description,
        model.MinimumSalary,
        model.MaximumSalary,
        model.Location,
        model.JobType,
        Deadline = model.Deadline.ToUniversalTime()
    };
}
