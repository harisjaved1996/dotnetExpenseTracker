using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spendwise.Data;
using spendwise.Models;
using spendwise.Services;

namespace spendwise.Controllers;

public class DashboardController : BaseController
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var authCheck = RequireAuth("User");
        if (authCheck != null)
            return authCheck;

        var model = new DashboardViewModel
        {
            CategoryCount = await _db.Categories.CountAsync(),
            TotalSpentThisMonth = 2_450.75m,
            MonthlyBudget = 4_000.00m,
            TotalExpensesThisMonth = 18,
            RecentExpenses =
            [
                new("Grocery Shopping",      "Food",          DateTime.Now.AddDays(0),  85.50m,  "Paid"),
                new("Uber Ride",             "Transport",     DateTime.Now.AddDays(-1), 24.00m,  "Paid"),
                new("Netflix Subscription",  "Entertainment", DateTime.Now.AddDays(-2), 15.99m,  "Paid"),
                new("Electricity Bill",      "Utilities",     DateTime.Now.AddDays(-3), 120.00m, "Pending"),
                new("New Headphones",        "Shopping",      DateTime.Now.AddDays(-5), 249.99m, "Pending"),
            ],
            CategorySpend =
            [
                new("Food",          620m, "#f5a623"),
                new("Transport",     310m, "#1e3a5f"),
                new("Entertainment", 185m, "#e74c3c"),
                new("Shopping",      490m, "#2ecc71"),
                new("Utilities",     845m, "#9b59b6"),
            ],
        };

        return View(model);
    }
}
