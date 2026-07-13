using Microsoft.AspNetCore.Mvc;
using SkillBridge.Web.Services;
using SkillBridge.Web.ViewModels;

namespace SkillBridge.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiService _api;

    public AccountController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (model.Type is not ("Applicant" or "Company"))
        {
            ModelState.AddModelError(nameof(model.Type), "Choose Applicant or Company.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.PostAsync<RegistrationApiResponse>("api/user/create", new
        {
            model.Name,
            model.Email,
            model.Password,
            model.Type
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return View(model);
        }

        TempData["Success"] = "Registration successful. You can now sign in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.PostAsync<LoginApiResponse>("api/user/login", new
        {
            model.Email,
            model.Password
        }, cancellationToken);

        if (!result.IsSuccess || result.Data is null || string.IsNullOrWhiteSpace(result.Data.AccessToken))
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Invalid email or password.");
            return View(model);
        }

        HttpContext.Session.SetString(SessionKeys.AccessToken, result.Data.AccessToken);
        HttpContext.Session.SetInt32(SessionKeys.UserId, result.Data.UserId);
        HttpContext.Session.SetString(SessionKeys.UserName, result.Data.Name);
        HttpContext.Session.SetString(SessionKeys.Email, result.Data.Email);
        HttpContext.Session.SetString(SessionKeys.UserType, result.Data.Type);

        TempData["Success"] = $"Welcome back, {result.Data.Name}.";
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return result.Data.Type == "Company"
            ? RedirectToAction("Dashboard", "Company")
            : RedirectToAction("Dashboard", "Applicant");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been signed out.";
        return RedirectToAction("Index", "Home");
    }
}
