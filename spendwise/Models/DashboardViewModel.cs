namespace spendwise.Models;

public class DashboardViewModel
{
    public int CategoryCount { get; init; }
    public decimal TotalSpentThisMonth { get; init; }
    public decimal MonthlyBudget { get; init; }
    public decimal TotalSavingsThisMonth => MonthlyBudget - TotalSpentThisMonth;
    public int TotalExpensesThisMonth { get; init; }
    public int BudgetUsedPercent => (int)Math.Min(TotalSpentThisMonth / MonthlyBudget * 100, 100);
    public IReadOnlyList<RecentExpenseRow> RecentExpenses { get; init; } = [];
    public IReadOnlyList<CategorySpendRow> CategorySpend { get; init; } = [];
}

public record RecentExpenseRow(string Description, string Category, DateTime Date, decimal Amount, string Status);
public record CategorySpendRow(string Category, decimal Amount, string Color);
