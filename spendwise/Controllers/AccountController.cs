using Microsoft.AspNetCore.Mvc;
using spendwise.Models;
using spendwise.Services;

namespace spendwise.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Login()
    {
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            var role = SessionHelper.GetRole(HttpContext.Session);
            return role == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _authService.AuthenticateAsync(model.Email, model.Password, model.Role);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email, password, or user type. Please try again.");
            return View(model);
        }

        SessionHelper.SetUser(HttpContext.Session, user, model.Role);

        var successMessage = model.Role == "Admin"
            ? $"Welcome back, {user.Name}! You are logged in as Admin."
            : $"Welcome back, {user.Name}! You are logged in as User.";

        TempData["SuccessMessage"] = successMessage;

        return model.Role == "Admin"
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Register()
    {
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            var role = SessionHelper.GetRole(HttpContext.Session);
            return role == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        ModelState.AddModelError(string.Empty, "Registration is not yet available — check back soon.");
        return View(model);
    }

    public IActionResult Logout()
    {
        SessionHelper.ClearSession(HttpContext.Session);
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Login");
    }
}
