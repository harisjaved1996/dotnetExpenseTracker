# Spec: Design Login Page

## Overview
Create a polished, branded login page for SpendWise. The navbar "Login" anchor currently points to `href="#"` with no real route. This step wires up a dedicated `AccountController` with GET and POST `Login` actions, a `LoginViewModel`, and a fully designed `Views/Account/Login.cshtml` view. Because ASP.NET Core Identity is not yet configured, the POST action validates the model and then returns the view with a "Authentication is not yet configured" model error — no real credential check is performed. The navbar "Login" link is updated to point at the real route via Tag Helpers. The page follows the navy/gold design language established in step 01.

## Depends on
- **Step 01 — Design Home / Landing Page** — `_Layout.cshtml`, CSS custom properties (`--sw-navy`, `--sw-gold`), `sw-btn-gold`, `sw-btn-outline`, and shared navbar/footer must already be in place.

## Routes (Controller Actions)
- `AccountController` → `Login` — GET — Returns the login page — anonymous
- `AccountController` → `Login` — POST — Validates the form; if `ModelState.IsValid` returns false re-renders with errors; if valid, adds a model-level error ("Authentication is not yet available — check back soon.") and re-renders the view — anonymous

## Database changes
No database changes.

## Models & ViewModels
- **New ViewModels:** `Models/LoginViewModel.cs` — contains `Email` (required, EmailAddress), `Password` (required, DataType.Password, minlength 6), `RememberMe` (bool), and `Role` (string, default `"User"`, value is either `"User"` or `"Admin"`) properties with appropriate `[Display]` annotations and data annotations for client-side validation

## Views
- **Create:** `Views/Account/Login.cshtml` — full login page with form, validation summary, and links to Register (placeholder) and Forgot Password (placeholder)
- **Modify:** `Views/Shared/_Layout.cshtml` — replace `href="#"` on the "Login" navbar anchor with `asp-controller="Account" asp-action="Login"`

## Files to change
- `spendwise/Views/Shared/_Layout.cshtml` — update the navbar "Login" anchor to use Tag Helpers

## Files to create
- `spendwise/Controllers/AccountController.cs` — new controller with GET and POST `Login` actions
- `spendwise/Models/LoginViewModel.cs` — ViewModel for the login form
- `spendwise/Views/Account/Login.cshtml` — the login page view

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
- Do not add `[Authorize]` — both actions are fully public (no auth gate)
- Do not scaffold or reference ASP.NET Core Identity — keep the POST action as a stub
- The POST action must be decorated with `[HttpPost]` and `[ValidateAntiForgeryToken]`
- The GET action must be a simple expression-bodied `return View();`
- Page `<title>` must be set via `ViewData["Title"]` — value: `"Login"`
- Wire up `_ValidationScriptsPartial.cshtml` in a `@section Scripts` block so client-side validation works

## View layout requirements

### Page structure (`Login.cshtml`)
- `ViewData["Title"] = "Login";`
- Page is centered, no full-width hero — use a single-column card layout
- Top of the page: small navy banner strip (same pattern as Terms/Privacy pages) with heading **"Welcome Back"** and subtext **"Sign in to your SpendWise account"**
- Card: white background, `border-radius: 12px`, `box-shadow` subtle, max-width ~480 px, horizontally centered with `mx-auto`, `my-5` vertical margin, `p-4 p-md-5` padding
- Card header (inside the card): SpendWise logo text in navy (`var(--sw-navy)`), centered, `fw-bold fs-4`, above the form

#### Role toggle (top of card, below the logo text, above the form fields)
- A pill-shaped toggle switcher with two segments: **"User"** (left) and **"Admin"** (right)
- Styled as a segmented control: outer container navy border, `border-radius: 50px`, full width of the card's inner content area
- Active segment: navy background (`var(--sw-navy)`), white text, `border-radius: 50px`
- Inactive segment: transparent background, navy text, no border
- Clicking a segment sets it as active (JS swaps the active class); clicking the already-active segment does nothing
- A hidden `<input type="hidden" asp-for="Role">` field is kept in sync with the active segment value (`"User"` or `"Admin"`) via vanilla JS in `wwwroot/js/site.js`
- Default active segment: **"User"**

- Form fields (inside the card):
  1. **Email Address** — `<input type="email">`, full width, Bootstrap `form-control`, label above, validation message below
  2. **Password** — `<input type="password">`, full width, Bootstrap `form-control`, label above, validation message below; include a toggle-visibility eye icon button (vanilla JS, `wwwroot/js/site.js`)
  3. **Remember Me** — Bootstrap `form-check` checkbox with label "Keep me signed in"
  4. **Forgot Password?** — right-aligned link below the password field, `href="#"` (no real route yet), gold color
- Validation summary: `asp-validation-summary="ModelOnly"` block styled with Bootstrap `alert alert-danger` — only shown when there are model-level errors (used for the POST stub message)
- Submit button: full-width, gold-filled (`sw-btn-gold` or `btn` equivalent), text **"Sign In"**
- Divider line below the button
- Footer of card: "Don't have an account?" text + **"Register"** link (`href="#"`, gold color)
- Anti-forgery token: `@Html.AntiForgeryToken()` inside the form (or use the Tag Helper `<form asp-action="Login" method="post">` which emits it automatically)

### Password visibility toggle (JS)
- Eye icon button positioned inside the password input (use Bootstrap `input-group` wrapper)
- On click: toggle `type` attribute between `"password"` and `"text"` on the input; swap icon between `bi-eye` and `bi-eye-slash`
- Implement in `wwwroot/js/site.js` — no inline JS in the view

### Role toggle (JS)
- Both "User" and "Admin" segments are `<button type="button">` elements inside a wrapper `<div>`
- On click: remove `active` class from all segments; add `active` class to the clicked segment; update the hidden `Role` input value to match the clicked segment's data attribute (`data-role="User"` or `data-role="Admin"`)
- Implement in `wwwroot/js/site.js` — no inline JS in the view

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `/Account/Login` loads a page titled "Login – SpendWise"
- [ ] The navbar "Login" button navigates to `/Account/Login` (no longer `href="#"`)
- [ ] The navbar and footer render identically to every other page
- [ ] The login form shows Email, Password, and Remember Me fields
- [ ] Submitting the form with an empty Email or Password shows inline validation errors without a page reload (client-side)
- [ ] Submitting a fully valid form (real email + password ≥ 6 chars) shows the "Authentication is not yet available" alert at the top of the card
- [ ] The role toggle renders with "User" active by default; clicking "Admin" makes it active and "User" inactive; clicking "User" switches back
- [ ] The hidden `Role` field value updates correctly when the active segment changes (verifiable via browser DevTools)
- [ ] The password visibility toggle switches the input type and icon correctly
- [ ] The "Forgot Password?" link is visible below the password field (href="#", no navigation)
- [ ] The "Register" link at the bottom of the card is visible (href="#", no navigation)
- [ ] The page is fully responsive — form card is readable on a 375 px viewport and on a 1440 px viewport
- [ ] No browser console errors on page load
