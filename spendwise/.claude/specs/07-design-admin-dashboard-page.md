# Spec: Design Admin Dashboard Page

## Overview
Create the admin-facing dashboard for SpendWise. This mirrors the structure of the user dashboard (step 06) but shows system-wide metrics and management tools relevant to an administrator. The page has its own `AdminController`, an `AdminDashboardViewModel` with admin-scoped stub data, and a `Views/Admin/Index.cshtml` view. The sidebar navigation reflects admin concerns (Users, Categories, Expenses) rather than personal ones. The same CSS classes established in step 06 (`sw-dashboard-wrapper`, `sw-sidebar`, `sw-topbar`, `sw-main-content`, `sw-stat-card`, `sw-dashboard-card`, `sw-avatar`) are reused without modification. Only `CategoryCount` is queried from the real DB; all other figures are hardcoded stubs. The page is publicly accessible at `/Admin` for now — auth gating is deferred to a later Identity step.

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS tokens, and shared styles must be in place.
- **Step 06 — User Dashboard Page** — all dashboard CSS classes (`sw-sidebar`, `sw-topbar`, etc.) and Chart.js JS init in `site.js` must already exist; this step adds no new CSS or JS.

## Routes (Controller Actions)
- `AdminController` → `Index` — GET — Queries `Categories` count from DB, builds `AdminDashboardViewModel` with stub data, returns the admin dashboard view — anonymous (auth gate deferred)

## Database changes
No database changes — the action reads the existing `Categories` DbSet (`CountAsync`) but adds no new tables or columns.

## Models & ViewModels
- **New ViewModels:** `Models/AdminDashboardViewModel.cs` — properties:
  - `int CategoryCount` — real value from `AppDbContext.Categories.CountAsync()`
  - `int TotalUsers` — stub (hardcoded `142`)
  - `int TotalExpenses` — stub (hardcoded `3_847`)
  - `decimal MonthlyVolume` — stub (hardcoded `48_230.50m`)
  - `IReadOnlyList<RecentUserRow> RecentUsers` — list of 5 stub rows
  - `IReadOnlyList<CategorySpendRow> CategorySpend` — reuse existing `CategorySpendRow` record from `DashboardViewModel.cs` for the chart (same record, no duplication needed)

- **New nested record type inside `AdminDashboardViewModel.cs`:**
  - `record RecentUserRow(string Name, string Email, string Role, DateTime JoinedAt, string Status)`

## Views
- **Create:** `Views/Admin/Index.cshtml` — full admin dashboard page with sidebar, stat cards, chart, recent users table, and quick actions

## Files to change
- *(no existing files need modification — all dashboard CSS and JS already exist from step 06)*

## Files to create
- `spendwise/Controllers/AdminController.cs` — `Index` GET action
- `spendwise/Models/AdminDashboardViewModel.cs` — ViewModel + `RecentUserRow` record
- `spendwise/Views/Admin/Index.cshtml` — the admin dashboard view

## New NuGet packages
No new packages — Chart.js already loaded per-page via `@section Scripts` in the user dashboard; admin view will do the same.

## Rules for implementation
- Follow ASP.NET Core MVC conventions — no Razor Pages, no Web API controllers
- Use EF Core async APIs (`CountAsync`, `ToListAsync`, etc.) for all DB calls
- Code-first migrations only — never edit generated migration files after applying them
- Validate at the controller boundary via `ModelState.IsValid`; trust EF internals below
- Keep controllers thin — move query logic to AppDbContext or a service if it exceeds ~20 lines
- File-scoped namespaces matching folder path (e.g. `namespace spendwise.Controllers;`)
- Bootstrap 5 utility classes preferred; custom CSS only when Bootstrap cannot do it
- All custom styles go in `wwwroot/css/site.css`; all custom scripts in `wwwroot/js/site.js`
- All views must extend `_Layout.cshtml`
- No npm, Webpack, TypeScript, or JS build tooling
- No XML doc comments on internal/private members
- Do not add `[Authorize]` — page is publicly accessible at this stage
- Do NOT add new CSS classes to `site.css` — reuse all existing dashboard classes from step 06
- Do NOT add new JS to `site.js` — the Chart.js init already guards with a null check and works for any page that has `#categoryChart`
- Chart.js must be loaded from CDN in `@section Scripts`, same pattern as `Views/Dashboard/Index.cshtml`
- `CategorySpendRow` record is already declared in `DashboardViewModel.cs` in the `spendwise.Models` namespace — reference it directly, do not redeclare it

## View layout requirements

### Overall structure
Identical outer shell to `Views/Dashboard/Index.cshtml`: `sw-dashboard-wrapper` flex row containing a desktop sidebar (`d-none d-lg-flex`), a mobile Bootstrap Offcanvas sidebar (`id="adminMobileSidebar"`), and a `sw-main-content` flex column.

### Sidebar
- Same visual style as user dashboard sidebar (`sw-sidebar`, navy bg, gold brand, white nav links)
- Brand: **SpendWise Admin** in gold at the top
- Navigation links (vertical list) — four links:
  - **Dashboard** — `bi-speedometer2` icon — active (`asp-controller="Admin" asp-action="Index"`)
  - **Users** — `bi-people` icon — `href="#"` placeholder
  - **Categories** — `bi-tags` icon — `href="#"` placeholder
  - **Expenses** — `bi-receipt` icon — `href="#"` placeholder
- No other links

### Top bar
- Same `sw-topbar` pattern as user dashboard
- Left: hamburger toggle (mobile, `data-bs-target="#adminMobileSidebar"`) + page title **"Admin Dashboard"** in navy
- Right: avatar circle **"SA"** (Super Admin initials) + name **"Super Admin"** + small red `badge bg-danger` label **"Admin"** next to the name + **"Logout"** button (`sw-btn-outline`, `href="#"`, `bi-box-arrow-right` icon)

### Page content area

#### Welcome header row
- Left: **"System Overview"** heading (`h5`, navy, `fw-semibold`) + subtext `"SpendWise Administration Panel"` in muted gray
- Right: today's date server-side (`DateTime.Now.ToString("dddd, MMMM d, yyyy")`)

#### Stat cards (4 cards, `row-cols-1 row-cols-md-2 row-cols-xl-4`)
Each uses `.sw-stat-card` — white bg, gold left border, radius, shadow.

| Card | Icon | Label | Value |
|---|---|---|---|
| Total Users | `bi-people-fill` | "Registered Users" | `Model.TotalUsers` |
| Total Expenses | `bi-receipt-cutoff` | "Total Expenses" | `Model.TotalExpenses` |
| Categories | `bi-tags-fill` | "Categories" | `Model.CategoryCount` (real DB) |
| Monthly Volume | `bi-currency-dollar` | "Monthly Volume" | `Model.MonthlyVolume.ToString("C")` |

Icons displayed large (`fs-1`) in gold.

#### Middle row: Chart (col-lg-7) + Quick Actions (col-lg-5)

**Spending by Category chart** — identical pattern to user dashboard:
- `sw-dashboard-card`, heading "Expense Distribution"
- `<canvas id="categoryChart">` with `data-labels`, `data-values`, `data-colors` attributes populated from `Model.CategorySpend` via `JsonSerializer.Serialize`
- Same 5 stub rows as user dashboard (Food, Transport, Entertainment, Shopping, Utilities)
- Chart.js doughnut, legend at bottom — works automatically via existing `site.js` init

**Admin Quick Actions panel**
- `sw-dashboard-card`, heading "Admin Actions"
- Vertical stack of buttons (full width each):
  - **"+ Add Category"** — gold-filled (`sw-btn-gold`), `href="#"` placeholder
  - **"Manage Users"** — gold-outline (`sw-btn-outline`), `href="#"` placeholder
  - **"View All Expenses"** — gold-outline, `href="#"` placeholder
  - **"Export Report"** — gold-outline with `bi-download` icon, `href="#"` placeholder
- System health strip below the buttons:
  - Label row: "System Status" left, green dot + **"Operational"** right
  - Three small metric rows (muted text, small font):
    - Database: Connected (green `bi-circle-fill` dot)
    - Last Backup: Yesterday at 2:00 AM
    - Uptime: 99.8%

#### Bottom row: Recent Users table (full width)
- `sw-dashboard-card`
- Header: "Recent Users" heading + "View All →" link (`href="#"`, gold)
- `table table-hover`, navy header row
- Columns: `#` | Name | Email | Role | Joined | Status
- Render `Model.RecentUsers` (5 stub rows):
  1. "Alice Johnson", alice@example.com, User, today -0 days, Active
  2. "Bob Smith", bob@example.com, User, today -1 days, Active
  3. "Carol White", carol@example.com, Admin, today -2 days, Active
  4. "David Brown", david@example.com, User, today -4 days, Suspended
  5. "Eva Martinez", eva@example.com, User, today -6 days, Active
- Dates via `DateTime.Now.AddDays(-N)`
- **Role** column: `"Admin"` shown as `badge` with navy bg (`style="background:var(--sw-navy)"`), `"User"` shown as `badge bg-secondary`
- **Status** column: `"Active"` → `badge bg-success`, `"Suspended"` → `badge bg-danger`

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Admin` loads a page titled "Admin Dashboard – SpendWise"
- [ ] The sidebar renders with four navigation links — Dashboard (active), Users, Categories, Expenses
- [ ] "SpendWise Admin" appears in gold at the top of the sidebar
- [ ] The top bar shows avatar "SA", name "Super Admin", red "Admin" badge, and a "Logout" button
- [ ] Clicking "Logout" does nothing (`href="#"`)
- [ ] The four stat cards display with correct stub values and gold icons
- [ ] The doughnut chart renders with five category slices and a legend
- [ ] The Admin Quick Actions panel shows four buttons and the system health strip
- [ ] The Recent Users table shows five rows with correct stub data, role badges, and status badges
- [ ] On desktop (≥ lg) the sidebar is visible alongside the main content
- [ ] On mobile (< lg) the sidebar is hidden; the hamburger opens the offcanvas overlay
- [ ] The page is fully responsive — readable on 375 px and 1440 px viewports
- [ ] No browser console errors on page load
- [ ] No new CSS classes were added to `site.css` (all reused from step 06)
