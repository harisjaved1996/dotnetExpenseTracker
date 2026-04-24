# Spec: Design Privacy Policy

## Overview
Create a dedicated Privacy Policy page for SpendWise. The footer "Privacy Policy" link currently points to `href="#"` with no real route. This step wires up a proper GET route, creates a Razor view that renders the privacy policy content, and updates the footer anchor in `_Layout.cshtml` to use the real Tag Helper URL. The Terms & Conditions page (step 02) also references the Privacy Policy with `href="#"` — that link will be updated here too. The page is fully public (no auth gate) and follows the same navy/gold design language established in step 01.

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS custom properties (`--sw-navy`, `--sw-gold`), and the shared footer must already be in place.
- **Step 02 — Design Terms and Conditions** — the Terms & Conditions view contains a "Data and Privacy" section that links to this page; that placeholder link must be updated.

## Routes (Controller Actions)
- `HomeController` → `PrivacyPolicy` — GET — Returns the Privacy Policy page — anonymous

## Database changes
No database changes.

## Models & ViewModels
No new models or ViewModels — the page is static content.

## Views
- **Create:** `Views/Home/PrivacyPolicy.cshtml` — full Privacy Policy page with headings and policy sections
- **Modify:** `Views/Shared/_Layout.cshtml` — replace `href="#"` on the "Privacy Policy" footer link with `asp-controller="Home" asp-action="PrivacyPolicy"`
- **Modify:** `Views/Home/TermsAndConditions.cshtml` — replace `href="#"` in the "Data and Privacy" section with `asp-controller="Home" asp-action="PrivacyPolicy"`

## Files to change
- `spendwise/Controllers/HomeController.cs` — add `PrivacyPolicy()` GET action
- `spendwise/Views/Shared/_Layout.cshtml` — update the footer "Privacy Policy" anchor to use Tag Helpers
- `spendwise/Views/Home/TermsAndConditions.cshtml` — update the "Data and Privacy" section link to use Tag Helpers

## Files to create
- `spendwise/Views/Home/PrivacyPolicy.cshtml` — the Privacy Policy page view

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
- Page `<title>` must be set via `ViewData["Title"]` — value: `"Privacy Policy"`

## View layout requirements

### Page structure (`PrivacyPolicy.cshtml`)
- `ViewData["Title"] = "Privacy Policy";`
- Top banner: navy background (`var(--sw-navy)`), white text, centered heading "Privacy Policy", small subtext showing effective date "Effective Date: January 1, 2026"
- Content area: white background, Bootstrap `container`, `py-5` vertical padding
- Use `<h2>` for section headings with gold left-border accent (CSS: `border-left: 4px solid var(--sw-gold); padding-left: 0.75rem;`) — reuse the existing `.sw-section-heading` class if already present in `site.css`, otherwise add it
- Use `<p>` for body text; readable line length — wrap content in `col-lg-9 mx-auto` column
- Sections to include (use realistic placeholder copy):
  1. **Introduction** — SpendWise is committed to protecting your personal information; overview of what this policy covers
  2. **Information We Collect** — account details (name, email), expense data you enter, and usage/log data
  3. **How We Use Your Information** — to provide and improve the service, send account-related emails, and comply with legal obligations
  4. **Data Storage and Security** — data stored on secure servers; industry-standard encryption; no guarantee of absolute security
  5. **Sharing of Information** — we do not sell your data; limited sharing with service providers under confidentiality agreements
  6. **Cookies and Tracking** — session cookies for authentication; no third-party advertising trackers
  7. **Your Rights** — right to access, correct, or delete your data; contact us to make a request
  8. **Changes to This Policy** — SpendWise may update this policy; continued use after changes = acceptance; check this page periodically
  9. **Contact Us** — email placeholder `privacy@spendwise.com`
- "Back to Home" link at the bottom using Tag Helper: `asp-controller="Home" asp-action="Index"`, styled as gold-filled button (`sw-btn-gold` class)
- "View Terms & Conditions" secondary link using Tag Helper: `asp-controller="Home" asp-action="TermsAndConditions"`, styled as gold-outline button (`sw-btn-gold-outline` class or `btn-outline` equivalent), placed next to the "Back to Home" button

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Home/PrivacyPolicy` (or the generated route) loads a page titled "Privacy Policy – SpendWise"
- [ ] The navbar and footer render identically to every other page
- [ ] Clicking "Privacy Policy" in the footer navigates to the Privacy Policy page (no longer `href="#"`)
- [ ] Clicking "Privacy Policy" link in the Terms & Conditions "Data and Privacy" section navigates to this page (no longer `href="#"`)
- [ ] All nine content sections are visible on the page
- [ ] The "Back to Home" button navigates back to the landing page
- [ ] The "View Terms & Conditions" button navigates to the Terms & Conditions page
- [ ] The page is fully responsive — readable on a 375 px viewport and on a 1440 px viewport
- [ ] No browser console errors on page load
