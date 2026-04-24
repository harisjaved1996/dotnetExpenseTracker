using Microsoft.AspNetCore.Mvc;
using spendwise.Models;

namespace spendwise.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        ModelState.AddModelError(string.Empty, "Login is not yet available — check back soon.");
        return View(model);
    }

    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        ModelState.AddModelError(string.Empty, "Registration is not yet available — check back soon.");
        return View(model);
    }
}
