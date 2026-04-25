using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using spendwise.Models;
using spendwise.Services;

namespace spendwise.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        if (IsAuthenticated)
        {
            return AuthRole == "Admin"
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult TermsAndConditions() => View();

    public IActionResult PrivacyPolicy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
