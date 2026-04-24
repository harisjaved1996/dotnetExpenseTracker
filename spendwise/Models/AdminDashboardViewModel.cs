namespace spendwise.Models;

public class AdminDashboardViewModel
{
    public int CategoryCount    { get; init; }
    public int TotalUsers       { get; init; }
    public int TotalExpenses    { get; init; }
    public decimal MonthlyVolume { get; init; }
    public IReadOnlyList<RecentUserRow> RecentUsers     { get; init; } = [];
    public IReadOnlyList<CategorySpendRow> CategorySpend { get; init; } = [];
}

public record RecentUserRow(string Name, string Email, string Role, DateTime JoinedAt, string Status);
