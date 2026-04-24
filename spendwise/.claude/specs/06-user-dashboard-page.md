# Spec: User Dashboard Page

## Overview
Create the main authenticated dashboard for SpendWise users. After login (not yet functional), users will land here to see a summary of their financial activity. This step builds the full dashboard UI: a collapsible left sidebar for app navigation, summary stat cards, a recent-expenses table, a spending-by-category chart (Chart.js via CDN), and quick-action buttons. Because no Expense entity exists yet, the expense-related figures are hardcoded stub values; only the `CategoryCount` stat is pulled from the real DB. The page is publicly accessible at `/Dashboard` for now — auth gating is deferred to a later Identity step.

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS custom properties (`--sw-navy`, `--sw-gold`), and shared component styles must be in place.
- **Step 04 — Design Login Page** — establishes `AccountController`; the dashboard "Sign Out" sidebar link will point back to `Account/Login` as a placeholder.

## Routes (Controller Actions)
- `DashboardController` → `Index` — GET — Queries `Categories` count from DB, builds `DashboardViewModel` with stub expense data, returns the dashboard view — anonymous (auth gate deferred)

## Database changes
No database changes — the action reads the existing `Categories` DbSet but adds no new tables or columns.

## Models & ViewModels
- **New ViewModels:** `Models/DashboardViewModel.cs` — properties:
  - `int CategoryCount` — real value from `AppDbContext.Categories.CountAsync()`
  - `decimal TotalSpentThisMonth` — stub value (hardcoded `2_450.75m`)
  - `decimal MonthlyBudget` — stub value (hardcoded `4_000.00m`)
  - `decimal TotalSavingsThisMonth` — stub value (hardcoded `MonthlyBudget - TotalSpentThisMonth`)
  - `int TotalExpensesThisMonth` — stub value (hardcoded `18`)
  - `IReadOnlyList<RecentExpenseRow> RecentExpenses` — list of 5 stub rows (see below)
  - `IReadOnlyList<CategorySpendRow> CategorySpend` — list of stub rows matching the chart (see below)

- **New nested record types inside `DashboardViewModel.cs`:**
  - `record RecentExpenseRow(string Description, string Category, DateTime Date, decimal Amount)`
  - `record CategorySpendRow(string Category, decimal Amount, string Color)`

## Views
- **Create:** `Views/Dashboard/Index.cshtml` — full dashboard page with sidebar, stat cards, chart, table, and quick actions

## Files to change
- `spendwise/Controllers/HomeController.cs` — no change needed
- *(no existing files need modification for this step)*

## Files to create
- `spendwise/Controllers/DashboardController.cs` — `Index` GET action
- `spendwise/Models/DashboardViewModel.cs` — ViewModel + nested record types
- `spendwise/Views/Dashboard/Index.cshtml` — the dashboard view

## New NuGet packages
No new packages — Chart.js loaded via CDN inside the view's `@section Scripts` block.

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
- Chart.js must be loaded from CDN (`https://cdn.jsdelivr.net/npm/chart.js`) inside the view's `@section Scripts` block, after the layout's script bundle
- All chart initialization JS must go in `wwwroot/js/site.js` — no inline `<script>` in the view; wire up via a `data-` attribute on the canvas element

## View layout requirements

### Overall structure
The view renders inside `_Layout.cshtml`'s `@RenderBody()`. Immediately inside, use a full-height flex container that splits into:
- **Left: sidebar** (fixed-width `240px` on ≥ lg, off-canvas slide-in on < lg)
- **Right: main content** (fills remaining width, scrollable)

The outer container must compensate for the fixed navbar height (`padding-top: 64px` already on `body` via `_Layout.cshtml`) and fill the viewport height using `min-height: calc(100vh - 64px)`.

### Sidebar
- Navy background (`var(--sw-navy)`), white text, `240px` wide on desktop, full-height
- Top section: **SpendWise** brand name in gold (`var(--sw-gold)`), `fw-bold fs-5`, centered — acts as the sidebar header
- Navigation links (vertical list, no bullets) — only two links:
  - **Dashboard** — `bi-speedometer2` icon — active state (gold left-border accent `border-left: 3px solid var(--sw-gold)` + slightly lighter navy bg)
  - **Expenses** — `bi-receipt` icon — `href="#"` placeholder (no route yet)
- No other nav links (Categories, Reports, Settings are deferred to later steps)
- No Sign Out link in the sidebar — logout is handled in the top bar (see below)
- On mobile (< lg): sidebar is hidden by default; a hamburger-style toggle button in the top bar opens it as a Bootstrap Offcanvas overlay

### Main content area (right of sidebar)
Light gray background (`#f4f6f9`), no top padding (the top bar handles that).

#### Top bar (full-width strip above the main content, inside the main area)
- White background, `box-shadow: 0 1px 4px rgba(0,0,0,0.08)`, `padding: 0.75rem 1.5rem`, `height: 60px`
- **Left side:** hamburger toggle button (`bi-list`, visible only on < lg) that opens the sidebar Offcanvas, followed by a breadcrumb or page title **"Dashboard"** in navy, `fw-semibold`
- **Right side:** user name **"Alex Morgan"** (hardcoded stub) displayed in navy `fw-medium`, preceded by a circle avatar with initials **"AM"** (gold border, navy initials, same pill style as in the sidebar spec pattern), followed by a **"Logout"** button — styled as a small gold-outline button (`sw-btn-outline`, `btn-sm`) with a `bi-box-arrow-right` icon — `href="#"` (no action yet, nothing happens on click)

#### Page content header (below the top bar)
- `padding: 2rem 2rem 0`
- Left: subtext **"Welcome back, Alex Morgan"** in muted gray
- Right: today's date displayed as e.g. `Thursday, April 24, 2026`, rendered server-side via `DateTime.Now.ToString("dddd, MMMM d, yyyy")`

#### Summary stat cards (4 cards in a row, stack to 2×2 on md, single column on sm)
Each card: white background, `border-radius: 10px`, subtle shadow, `p-4`, left gold border accent (`border-left: 4px solid var(--sw-gold)`).

| Card | Icon | Label | Value source |
|---|---|---|---|
| Total Spent (Month) | `bi-wallet2` | "Spent This Month" | `Model.TotalSpentThisMonth` formatted as currency |
| Monthly Budget | `bi-piggy-bank` | "Monthly Budget" | `Model.MonthlyBudget` formatted as currency |
| Savings | `bi-arrow-up-circle` | "Saved This Month" | `Model.TotalSavingsThisMonth` formatted as currency |
| Expense Count | `bi-list-check` | "Transactions" | `Model.TotalExpensesThisMonth` |

The icon for each card is displayed large (`fs-1`) in gold (`var(--sw-gold)`).

#### Middle row: Chart (left 7 cols) + Quick Actions (right 5 cols)

**Spending by Category chart (Chart.js doughnut)**
- White card, `border-radius: 10px`, subtle shadow, `p-4`
- Heading: "Spending by Category"
- A `<canvas id="categoryChart">` element with `data-labels` and `data-values` attributes populated from `Model.CategorySpend` (JSON-serialized via `System.Text.Json` in the view using `@Html.Raw(Json.Serialize(...))` — assign to the `data-` attributes)
- Chart.js reads those attributes in `site.js` and renders a doughnut chart
- Stub `CategorySpend` data (5 rows): Food $620, Transport $310, Entertainment $185, Shopping $490, Utilities $845; colors: `#f5a623`, `#1e3a5f`, `#e74c3c`, `#2ecc71`, `#9b59b6`
- Legend displayed below the chart (Chart.js built-in legend, position `bottom`)

**Quick Actions panel**
- White card, `border-radius: 10px`, subtle shadow, `p-4`
- Heading: "Quick Actions"
- Vertical stack of action buttons (full width each):
  - **"+ Add Expense"** — gold-filled (`sw-btn-gold`), `href="#"` placeholder
  - **"View All Expenses"** — gold-outline (`sw-btn-outline`), `href="#"` placeholder
  - **"Manage Categories"** — gold-outline (`sw-btn-outline`), `href="#"` placeholder
  - **"Download Report"** — gold-outline (`sw-btn-outline`), `href="#"` placeholder
- Budget progress bar below the buttons:
  - Label: "Budget Used" with percentage text right-aligned (e.g. "61%")
  - Bootstrap `progress` bar, gold fill (`background: var(--sw-gold)`), height `10px`
  - Width calculated server-side: `(TotalSpentThisMonth / MonthlyBudget * 100)` clamped to 100

#### Bottom row: Recent Expenses table (full width)
- White card, `border-radius: 10px`, subtle shadow, `p-4`
- Heading: "Recent Expenses" with a "View All →" link (`href="#"`, gold) on the right
- Bootstrap `table table-hover`, header row navy background white text
- Columns: `#` | Description | Category | Date | Amount | Status
- Render `Model.RecentExpenses` (5 stub rows — see below); Amount right-aligned, formatted as currency
- "Status" column shows a Bootstrap badge: `"Paid"` (green `bg-success`) or `"Pending"` (yellow `bg-warning text-dark`) — alternate in stub data
- Stub `RecentExpenses` rows:
  1. "Grocery Shopping", Food, today -0 days, $85.50, Paid
  2. "Uber Ride", Transport, today -1 days, $24.00, Paid
  3. "Netflix Subscription", Entertainment, today -2 days, $15.99, Paid
  4. "Electricity Bill", Utilities, today -3 days, $120.00, Pending
  5. "New Headphones", Shopping, today -5 days, $249.99, Pending

  Dates are rendered as relative `DateTime.Now.AddDays(-N)` values so they always look current.

### CSS additions (`wwwroot/css/site.css`)
- `.sw-sidebar` — navy bg, fixed height, flex column, transition for mobile slide-in
- `.sw-sidebar .nav-link` — white text, padding, hover state (slightly lighter navy bg)
- `.sw-sidebar .nav-link.active` — gold left border (`border-left: 3px solid var(--sw-gold)`), slightly lighter navy bg
- `.sw-stat-card` — white bg, border-left gold accent, border-radius, shadow
- `.sw-main-content` — light gray bg, flex-grow, overflow-y auto

### JS additions (`wwwroot/js/site.js`)
- On `DOMContentLoaded`, find `<canvas id="categoryChart">`, read `data-labels` and `data-values` attributes (parse as JSON), initialize a Chart.js doughnut chart
- Guard with a null check so this code does not error on pages without the canvas

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Dashboard` loads a page titled "Dashboard – SpendWise"
- [ ] The sidebar renders with exactly two navigation links — "Dashboard" (active, gold-border) and "Expenses" (inactive)
- [ ] The SpendWise brand name appears in gold at the top of the sidebar
- [ ] The four summary stat cards display with correct stub values and gold icons
- [ ] The doughnut chart renders with five category slices and a legend
- [ ] The Quick Actions panel shows four buttons and the budget progress bar
- [ ] The budget progress bar width reflects the correct stub percentage (~61%)
- [ ] The Recent Expenses table shows five rows with correct stub data, currency formatting, and Paid/Pending badges
- [ ] On a desktop viewport (≥ lg) the sidebar is visible alongside the main content
- [ ] On a mobile viewport (< lg) the sidebar is hidden; a toggle button opens it as an off-canvas overlay
- [ ] The top bar shows the user avatar ("AM"), name "Alex Morgan", and a "Logout" button on the right
- [ ] Clicking the "Logout" button does nothing (href="#")
- [ ] The hamburger toggle in the top bar opens the sidebar as an Offcanvas overlay on mobile (< lg)
- [ ] The page is fully responsive — readable on 375 px and 1440 px viewports
- [ ] No browser console errors on page load
