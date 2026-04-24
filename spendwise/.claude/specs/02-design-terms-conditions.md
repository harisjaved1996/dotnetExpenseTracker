# Spec: Design Terms and Conditions

## Overview
Create a dedicated Terms & Conditions page for SpendWise. The footer "Terms & Conditions" link currently points to `href="#"` with no real route. This step wires up a proper GET route, creates a Razor view that renders the legal copy, and updates the footer anchor in `_Layout.cshtml` to use the real Tag Helper URL. The page is fully public (no auth gate) and follows the same navy/gold design language established in step 01.

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS custom properties (`--sw-navy`, `--sw-gold`), and the shared footer must already be in place.

## Routes (Controller Actions)
- `HomeController` → `TermsAndConditions` — GET — Returns the Terms & Conditions page — anonymous

## Database changes
No database changes.

## Models & ViewModels
No new models or ViewModels — the page is static content.

## Views
- **Create:** `Views/Home/TermsAndConditions.cshtml` — full Terms & Conditions page with headings and legal sections
- **Modify:** `Views/Shared/_Layout.cshtml` — replace `href="#"` on the "Terms &amp; Conditions" footer link with `asp-controller="Home" asp-action="TermsAndConditions"`

## Files to change
- `spendwise/Controllers/HomeController.cs` — add `TermsAndConditions()` GET action
- `spendwise/Views/Shared/_Layout.cshtml` — update the footer "Terms & Conditions" anchor to use Tag Helpers

## Files to create
- `spendwise/Views/Home/TermsAndConditions.cshtml` — the Terms & Conditions page view

## New NuGet packages
No new packages.

## Rules for implementation
- Follow ASP.NET Core MVC conventions — no Razor Pages, no Web API controllers
- Use EF Core async APIs (`ToListAsync`, `FirstOrDefaultAsync`, etc.) for all DB calls
- Code-first migrations only — never edit generated migration files after applying them
- Validate at the controller boundary via `ModelState.IsValid`; trust EF internals below
- Keep controllers thin — move query logic to AppDbContext or a service if it exceeds ~20 lines
- File-scoped namespaces matching folder path (e.g. `namespace spendwise.Controllers;`)
- Bootstrap 5 utility classes preferred; custom CSS only when Bootstrap cannot do it
- All custom styles go in `wwwroot/css/site.css`; all custom scripts in `wwwroot/js/site.js`
- All views must extend `_Layout.cshtml`
- No npm, Webpack, TypeScript, or JS build tooling
- No XML doc comments on internal/private members
- Do not add `[Authorize]` — this page is fully public
- The action method must be a simple expression-bodied `return View();` — no DB calls needed
- Page `<title>` must be set via `ViewData["Title"]` — value: `"Terms & Conditions"`

## View layout requirements

### Page structure (`TermsAndConditions.cshtml`)
- `ViewData["Title"] = "Terms & Conditions";`
- Top banner: navy background (`var(--sw-navy)`), white text, centered heading "Terms & Conditions", small subtext showing effective date "Effective Date: January 1, 2026"
- Content area: white background, Bootstrap `container`, `py-5` vertical padding
- Use `<h2>` for section headings with gold left-border accent (CSS: `border-left: 4px solid var(--sw-gold); padding-left: 0.75rem;`)
- Use `<p>` for body text; readable line length — wrap content in `col-lg-9 mx-auto` column
- Sections to include (use realistic placeholder copy):
  1. **Acceptance of Terms** — by using SpendWise you agree to these terms
  2. **Use of the Service** — personal, non-commercial use only; no misuse
  3. **User Accounts** — user responsibility for credentials and activity (future feature note)
  4. **Data and Privacy** — link to Privacy Policy (use `href="#"` — no real route yet)
  5. **Intellectual Property** — SpendWise branding and content ownership
  6. **Limitation of Liability** — service provided as-is, no warranty
  7. **Changes to Terms** — SpendWise may update terms; continued use = acceptance
  8. **Contact Us** — email placeholder `legal@spendwise.com`
- "Back to Home" link at the bottom using Tag Helper: `asp-controller="Home" asp-action="Index"`, styled as gold-filled button (`sw-btn-gold` class)

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Home/TermsAndConditions` (or the generated route) loads a page titled "Terms & Conditions – SpendWise"
- [ ] The navbar and footer render identically to every other page
- [ ] Clicking "Terms & Conditions" in the footer navigates to the Terms & Conditions page (no longer `href="#"`)
- [ ] All eight content sections are visible on the page
- [ ] The "Back to Home" button navigates back to the landing page
- [ ] The page is fully responsive — readable on a 375 px viewport and on a 1440 px viewport
- [ ] No browser console errors on page load
