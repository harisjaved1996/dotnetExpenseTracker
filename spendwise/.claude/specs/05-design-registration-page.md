# Spec: Design Registration Page

## Overview
Create a polished, branded registration page for SpendWise. The navbar "Register" anchor and the login page's "Register" link both currently point to `href="#"` with no real route. This step adds GET and POST `Register` actions to the existing `AccountController`, a `RegisterViewModel`, and a fully designed `Views/Account/Register.cshtml` view. Because ASP.NET Core Identity is not yet configured, the POST action validates the model and then returns the view with a stub message — no user is actually created. The navbar "Register" link and the login page "Register" link are both updated to use Tag Helpers. The page follows the same navy/gold design language and card layout established in step 04 (Login).

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS custom properties, `sw-btn-gold`, `sw-btn-outline`, navbar, and footer must already be in place.
- **Step 04 — Design Login Page** — `AccountController` and `Views/Account/` folder must already exist; the login card pattern is reused for visual consistency.

## Routes (Controller Actions)
- `AccountController` → `Register` — GET — Returns the registration page — anonymous
- `AccountController` → `Register` — POST — Validates the form; if `ModelState.IsValid` is false re-renders with errors; if valid, adds a model-level error ("Registration is not yet available — check back soon.") and re-renders the view — anonymous

## Database changes
No database changes.

## Models & ViewModels
- **New ViewModels:** `Models/RegisterViewModel.cs` — contains the following properties with `[Display]`, `[Required]`, and appropriate validation annotations:
  - `FullName` — required, `[StringLength(100, MinimumLength = 2)]`, display "Full Name"
  - `Email` — required, `[EmailAddress]`, display "Email Address"
  - `Password` — required, `[DataType(DataType.Password)]`, `[StringLength(100, MinimumLength = 6)]`, display "Password"
  - `ConfirmPassword` — required, `[DataType(DataType.Password)]`, `[Compare("Password", ErrorMessage = "Passwords do not match.")]`, display "Confirm Password"
  - `Role` — `string`, default `"User"`, value is either `"User"` or `"Admin"` (same pattern as `LoginViewModel.Role`)
  - `AgreeToTerms` — `bool`, `[Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms & Conditions.")]`, display "I agree to the Terms & Conditions"

## Views
- **Create:** `Views/Account/Register.cshtml` — full registration page with form, validation summary, role toggle, password strength meter, and link back to Login
- **Modify:** `Views/Shared/_Layout.cshtml` — replace `href="#"` on the "Register" navbar anchor with `asp-controller="Account" asp-action="Register"`
- **Modify:** `Views/Account/Login.cshtml` — replace `href="#"` on the "Register" card-footer link with `asp-controller="Account" asp-action="Register"`

## Files to change
- `spendwise/Controllers/AccountController.cs` — add GET and POST `Register` actions
- `spendwise/Views/Shared/_Layout.cshtml` — update navbar "Register" anchor to use Tag Helpers
- `spendwise/Views/Account/Login.cshtml` — update the card-footer "Register" link to use Tag Helpers

## Files to create
- `spendwise/Models/RegisterViewModel.cs` — ViewModel for the registration form
- `spendwise/Views/Account/Register.cshtml` — the registration page view

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
- Do not add `[Authorize]` — both actions are fully public
- Do not scaffold or reference ASP.NET Core Identity — keep the POST action as a stub
- The POST action must be decorated with `[HttpPost]` and `[ValidateAntiForgeryToken]`
- The GET action must be a simple expression-bodied `return View();`
- Page `<title>` must be set via `ViewData["Title"]` — value: `"Register"`
- Wire up `_ValidationScriptsPartial.cshtml` in a `@section Scripts` block so client-side validation works

## View layout requirements

### Page structure (`Register.cshtml`)
- `ViewData["Title"] = "Register";`
- Page is centered, no full-width hero — same single-column card layout as the login page
- Top of the page: small navy banner strip with heading **"Create Your Account"** and subtext **"Start tracking your expenses with SpendWise"**
- Card: white background, `border-radius: 12px`, subtle `box-shadow`, max-width ~480 px, horizontally centered with `mx-auto`, `my-5` vertical margin, `p-4 p-md-5` padding
- Card header (inside the card): SpendWise logo text in navy (`var(--sw-navy)`), centered, `fw-bold fs-4`, above the role toggle

#### Role toggle (top of card, below the logo text, above the form fields)
- Identical pill-shaped segmented control to the one on the login page: **"User"** (left) / **"Admin"** (right)
- Active segment: navy background, white text; inactive: transparent, navy text
- A hidden `<input type="hidden" asp-for="Role">` kept in sync via the same JS pattern used on the login page
- Default active segment: **"User"**

#### Form fields (inside the card, below the role toggle)
1. **Full Name** — `<input type="text">`, full width, Bootstrap `form-control`, label above, validation message below
2. **Email Address** — `<input type="email">`, full width, Bootstrap `form-control`, label above, validation message below
3. **Password** — `<input type="password">`, full width, Bootstrap `form-control`, label above, validation message below; password visibility toggle eye icon (same `input-group` pattern as login); password strength meter below the input (see below)
4. **Confirm Password** — `<input type="password">`, full width, Bootstrap `form-control`, label above, validation message below; password visibility toggle eye icon
5. **Agree to Terms** — Bootstrap `form-check` checkbox; label text: `"I agree to the "` + inline Tag Helper link to `HomeController → TermsAndConditions` (opens in new tab, gold color) + `" and "` + inline Tag Helper link to `HomeController → PrivacyPolicy` (opens in new tab, gold color); validation message below

#### Password strength meter
- A thin progress-bar strip rendered immediately below the Password input (outside the `input-group`)
- Four strength levels: **Weak** (red), **Fair** (orange), **Good** (yellow-green), **Strong** (green)
- Width animates from 0 → 25% / 50% / 75% / 100% as the user types based on a simple JS heuristic: length ≥ 6 = Weak, + uppercase = Fair, + number = Good, + special char = Strong
- Label text next to the bar reflects the current level (e.g. "Weak", "Strong")
- Implement entirely in `wwwroot/js/site.js` — no inline JS in the view
- Use Bootstrap `progress` component; color classes swapped by JS (`bg-danger`, `bg-warning`, `bg-info`, `bg-success`)

#### Below the form
- Validation summary: `asp-validation-summary="ModelOnly"` block styled as Bootstrap `alert alert-danger` — only shown when model-level errors exist (stub POST message)
- Submit button: full-width, gold-filled, text **"Create Account"**
- Divider line below the button
- Card footer: "Already have an account?" text + **"Sign In"** link using Tag Helpers `asp-controller="Account" asp-action="Login"`, gold color

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Account/Register` loads a page titled "Register – SpendWise"
- [ ] The navbar "Register" button navigates to `/Account/Register` (no longer `href="#"`)
- [ ] The "Register" link at the bottom of the Login card navigates to `/Account/Register` (no longer `href="#"`)
- [ ] The navbar and footer render identically to every other page
- [ ] The registration form shows Full Name, Email, Password, Confirm Password, and Agree to Terms fields
- [ ] Submitting the form with any required field empty shows inline client-side validation errors without a page reload
- [ ] Submitting with mismatched passwords shows the "Passwords do not match" error inline
- [ ] Submitting without checking "Agree to Terms" shows the checkbox validation error
- [ ] Submitting a fully valid form shows the "Registration is not yet available" alert at the top of the card
- [ ] The role toggle renders with "User" active by default; clicking "Admin" activates it; clicking "User" switches back
- [ ] The hidden `Role` field value updates correctly (verifiable via browser DevTools)
- [ ] The password visibility toggle works on both Password and Confirm Password inputs independently
- [ ] The password strength meter updates as the user types in the Password field (Weak → Fair → Good → Strong)
- [ ] The Terms & Conditions and Privacy Policy links in the checkbox label open the correct pages
- [ ] The "Sign In" card-footer link navigates to `/Account/Login`
- [ ] The page is fully responsive — readable on a 375 px viewport and on a 1440 px viewport
- [ ] No browser console errors on page load
