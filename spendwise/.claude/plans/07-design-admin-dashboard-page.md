# Implementation Plan: Design Admin Dashboard Page (Spec 07)

## Context
Implementing the SpendWise admin dashboard as defined in `.claude/specs/07-design-admin-dashboard-page.md`. This is a UI-only design step that mirrors step 06 (User Dashboard) but targets administrators. No auth is in place yet — the page is publicly accessible at `/Admin`. Only `CategoryCount` is queried from the real DB; all other figures are hardcoded stubs. All dashboard CSS classes and the Chart.js JS init already exist from step 06 — this step adds **no new CSS and no new JS**.

## Existing assets to reuse (do not recreate)
| Asset | Location |
|---|---|
| CSS classes | `wwwroot/css/site.css` — `sw-dashboard-wrapper`, `sw-sidebar`, `sw-sidebar-brand`, `sw-topbar`, `sw-main-content`, `sw-stat-card`, `sw-dashboard-card`, `sw-avatar` |
| Chart.js init | `wwwroot/js/site.js` — reads `#categoryChart` `data-labels/values/colors` attributes; null-guarded |
| `CategorySpendRow` record | `Models/DashboardViewModel.cs` — already in `spendwise.Models` namespace; do not redeclare |
| User dashboard pattern | `Views/Dashboard/Index.cshtml` — copy outer shell structure, adjust content |

## Files to create (3 total, no existing files modified)

---

### 1. `spendwise/Models/AdminDashboardViewModel.cs`

**Namespace:** `spendwise.Models`

**Contents:**
```csharp
namespace spendwise.Models;

public class AdminDashboardViewModel
{
    public int CategoryCount   { get; init; }
    public int TotalUsers      { get; init; }
    public int TotalExpenses   { get; init; }
    public decimal MonthlyVolume { get; init; }
    public IReadOnlyList<RecentUserRow> RecentUsers   { get; init; } = [];
    public IReadOnlyList<CategorySpendRow> CategorySpend { get; init; } = [];
}

public record RecentUserRow(string Name, string Email, string Role, DateTime JoinedAt, string Status);
```

> `CategorySpendRow` is already declared in `DashboardViewModel.cs` — reference it, do not redeclare.

---

### 2. `spendwise/Controllers/AdminController.cs`

**Namespace:** `spendwise.Controllers`  
**Usings:** `Microsoft.AspNetCore.Mvc`, `Microsoft.EntityFrameworkCore`, `spendwise.Data`, `spendwise.Models`

**Action:** `public async Task<IActionResult> Index()`

**Stub data to hardcode:**
- `TotalUsers = 142`
- `TotalExpenses = 3_847`
- `MonthlyVolume = 48_230.50m`
- `CategoryCount = await _db.Categories.CountAsync()`

**RecentUsers (5 rows):**
| Name | Email | Role | Days ago | Status |
|---|---|---|---|---|
| Alice Johnson | alice@example.com | User | 0 | Active |
| Bob Smith | bob@example.com | User | -1 | Active |
| Carol White | carol@example.com | Admin | -2 | Active |
| David Brown | david@example.com | User | -4 | Suspended |
| Eva Martinez | eva@example.com | User | -6 | Active |

**CategorySpend (5 rows, same as user dashboard):**
Food $620 `#f5a623`, Transport $310 `#1e3a5f`, Entertainment $185 `#e74c3c`, Shopping $490 `#2ecc71`, Utilities $845 `#9b59b6`

---

### 3. `spendwise/Views/Admin/Index.cshtml`

**Model:** `@model spendwise.Models.AdminDashboardViewModel`  
**Using:** `@using System.Text.Json`  
**Title:** `ViewData["Title"] = "Admin Dashboard";`

#### Top-level structure (mirrors `Views/Dashboard/Index.cshtml`)
```
<div class="sw-dashboard-wrapper">
    <!-- Desktop sidebar (d-none d-lg-flex) -->
    <!-- Mobile offcanvas (id="adminMobileSidebar") -->
    <div class="sw-main-content">
        <!-- Top bar -->
        <!-- Page content (p-4) -->
    </div>
</div>
@section Scripts { <script src="https://cdn.jsdelivr.net/npm/chart.js"></script> }
```

#### Desktop sidebar (`d-none d-lg-flex flex-column sw-sidebar`)
- Brand: `<span class="sw-sidebar-brand">SpendWise Admin</span>`
- 4 nav links:
  - Dashboard — `bi-speedometer2` — `active` — `asp-controller="Admin" asp-action="Index"`
  - Users — `bi-people` — `href="#"`
  - Categories — `bi-tags` — `href="#"`
  - Expenses — `bi-receipt` — `href="#"`

#### Mobile offcanvas (`id="adminMobileSidebar"`)
- Same brand + 4 nav links as desktop sidebar
- Bootstrap Offcanvas component with close button (`btn-close-white`)

#### Top bar (`.sw-topbar`)
- **Left:** hamburger `<button data-bs-toggle="offcanvas" data-bs-target="#adminMobileSidebar">` (visible `d-lg-none`) + `<span>"Admin Dashboard"</span>` in navy
- **Right:** `<span class="sw-avatar">SA</span>` + `"Super Admin"` text + `<span class="badge bg-danger">Admin</span>` + Logout `<a href="#" class="sw-btn-outline">` with `bi-box-arrow-right` icon

#### Welcome header row
- Left: `<h5>"System Overview"</h5>` + `<p class="text-muted">"SpendWise Administration Panel"</p>`
- Right: `@DateTime.Now.ToString("dddd, MMMM d, yyyy")`

#### Stat cards (`row row-cols-1 row-cols-md-2 row-cols-xl-4 g-4 mb-4`)
Each card uses `.sw-stat-card` with `d-flex justify-content-between`:

| # | Icon | Label | Value expression |
|---|---|---|---|
| 1 | `bi-people-fill` | "Registered Users" | `@Model.TotalUsers` |
| 2 | `bi-receipt-cutoff` | "Total Expenses" | `@Model.TotalExpenses` |
| 3 | `bi-tags-fill` | "Categories" | `@Model.CategoryCount` |
| 4 | `bi-currency-dollar` | "Monthly Volume" | `@Model.MonthlyVolume.ToString("C")` |

Icons: `class="fs-1"`, `style="color: var(--sw-gold);"`.

#### Middle row (`row g-4 mb-4`)

**Chart card (`col-lg-7`, `.sw-dashboard-card`)**
- Heading: "Expense Distribution"
- Serialize chart data in `@{ }` block at top of view:
  ```razor
  var labelsJson = JsonSerializer.Serialize(Model.CategorySpend.Select(c => c.Category));
  var valuesJson = JsonSerializer.Serialize(Model.CategorySpend.Select(c => c.Amount));
  var colorsJson = JsonSerializer.Serialize(Model.CategorySpend.Select(c => c.Color));
  ```
- Canvas: `<canvas id="categoryChart" data-labels="@labelsJson" data-values="@valuesJson" data-colors="@colorsJson" style="max-height:280px;">`
- Razor default HTML-encoding of `@labelsJson` in an attribute is safe — browser decodes `&quot;` back to `"` before `JSON.parse()` in `site.js`

**Admin Actions card (`col-lg-5`, `.sw-dashboard-card d-flex flex-column gap-3`)**
- Heading: "Admin Actions"
- 4 buttons (full width, stacked):
  1. `sw-btn-gold` — "+ Add Category" — `href="#"`
  2. `sw-btn-outline` — "Manage Users" — `href="#"`
  3. `sw-btn-outline` — "View All Expenses" — `href="#"`
  4. `sw-btn-outline` — `bi-download` "Export Report" — `href="#"`
- System health strip (`mt-2`, small text):
  ```
  System Status                          [green dot] Operational
  ─────────────────────────────────────────────────
  Database        [bi-circle-fill text-success] Connected
  Last Backup     Yesterday at 2:00 AM
  Uptime          99.8%
  ```
  Implement as a `<div class="mt-3 small">` with `d-flex justify-content-between` rows.

#### Recent Users table (full width, `.sw-dashboard-card`)
- Header: `"Recent Users"` + `"View All →"` link gold `href="#"`
- `table table-hover align-middle mb-0` inside `div.table-responsive`
- `<thead style="background-color: var(--sw-navy); color: #fff;">`
- Columns: `#` | Name | Email | Role | Joined | Status
- Loop `@foreach (var user in Model.RecentUsers)`:
  - **Role badge:** `@if (user.Role == "Admin")` → `<span class="badge" style="background:var(--sw-navy);">Admin</span>` else → `<span class="badge bg-secondary">User</span>`
  - **Status badge:** `@if (user.Status == "Active")` → `<span class="badge bg-success">Active</span>` else → `<span class="badge bg-danger">Suspended</span>`
  - **Joined date:** `@user.JoinedAt.ToString("MMM d, yyyy")`

---

## Implementation order
1. `AdminDashboardViewModel.cs` — no dependencies
2. `AdminController.cs` — depends on ViewModel
3. `Views/Admin/Index.cshtml` — depends on controller + ViewModel; create `Views/Admin/` folder first

## Verification checklist
1. `dotnet build` — zero errors, zero warnings
2. Navigate to `http://localhost:5108/Admin` — page title "Admin Dashboard – SpendWise"
3. Sidebar: 4 links, "SpendWise Admin" brand in gold, Dashboard active
4. Top bar: "SA" avatar, "Super Admin", red "Admin" badge, Logout button (no action)
5. Hamburger opens offcanvas sidebar on mobile (< lg)
6. 4 stat cards with correct values and gold icons
7. Doughnut chart renders with 5 slices and bottom legend
8. Admin Actions: 4 buttons + system health strip visible
9. Recent Users table: 5 rows, role/status badges correct
10. No browser console errors
11. `site.css` unchanged (no new classes added)
