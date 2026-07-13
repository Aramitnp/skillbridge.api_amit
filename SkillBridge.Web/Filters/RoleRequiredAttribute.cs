using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillBridge.Web.Services;

namespace SkillBridge.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RoleRequiredAttribute : ActionFilterAttribute
{
    private readonly string _role;

    public RoleRequiredAttribute(string role)
    {
        _role = role;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (string.IsNullOrWhiteSpace(session.GetString(SessionKeys.AccessToken)))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new
            {
                returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString
            });
            return;
        }

        if (!string.Equals(session.GetString(SessionKeys.UserType), _role, StringComparison.Ordinal))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}
