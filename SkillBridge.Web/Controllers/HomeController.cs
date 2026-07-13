using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Web.Models;
using SkillBridge.Web.Services;
using SkillBridge.Web.ViewModels;

namespace SkillBridge.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApiService _api;

    public HomeController(ApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _api.GetAsync<PaginatedJobResponse>(
            "api/jobs?pageNumber=1&pageSize=3",
            cancellationToken);

        return View(new HomeViewModel
        {
            RecentJobs = result.Data?.Items ?? [],
            Notice = result.IsSuccess ? null : result.Error
        });
    }

    public IActionResult AccessDenied() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
