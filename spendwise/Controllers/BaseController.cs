using Microsoft.AspNetCore.Mvc;
using spendwise.Services;

namespace spendwise.Controllers;

public class BaseController : Controller
{
    protected bool IsAuthenticated => SessionHelper.IsAuthenticated(HttpContext.Session);
    protected string? AuthRole => SessionHelper.GetRole(HttpContext.Session);

    protected IActionResult RequireAuth(string? requiredRole = null)
    {
        if (!IsAuthenticated)
            return RedirectToAction("Login", "Account");

        if (requiredRole != null && AuthRole != requiredRole)
            return RedirectToAction("Index", "Home");

        return null!;
    }
}
