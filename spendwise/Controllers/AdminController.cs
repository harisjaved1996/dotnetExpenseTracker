using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spendwise.Data;
using spendwise.Models;

namespace spendwise.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            CategoryCount = await _db.Categories.CountAsync(),
            TotalUsers    = 142,
            TotalExpenses = 3_847,
            MonthlyVolume = 48_230.50m,
            RecentUsers =
            [
                new("Alice Johnson", "alice@example.com", "User",  DateTime.Now.AddDays(0),  "Active"),
                new("Bob Smith",     "bob@example.com",   "User",  DateTime.Now.AddDays(-1), "Active"),
                new("Carol White",   "carol@example.com", "Admin", DateTime.Now.AddDays(-2), "Active"),
                new("David Brown",   "david@example.com", "User",  DateTime.Now.AddDays(-4), "Suspended"),
                new("Eva Martinez",  "eva@example.com",   "User",  DateTime.Now.AddDays(-6), "Active"),
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
