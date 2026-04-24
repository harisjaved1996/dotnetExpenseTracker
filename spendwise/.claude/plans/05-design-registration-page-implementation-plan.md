# Implementation Plan: Design Registration Page (Spec 05)

## Context

Spec 05 adds the full registration page to SpendWise. It depends on Step 04 (Login page), but exploration of the repo revealed that `AccountController`, `Views/Account/`, `LoginViewModel`, and `Views/Account/Login.cshtml` do **not yet exist**. This plan therefore bootstraps both the Login prerequisites and the Registration page in one pass.

The POST action is a stub — no Identity, no user creation. It validates the model, then renders an alert saying registration is not yet available.

---

## Files to Create

### 1. `spendwise/Models/LoginViewModel.cs`
Prerequisite ViewModel for step 04. Properties:
- `Email` — `[Required]`, `[EmailAddress]`, display "Email Address"
- `Password` — `[Required]`, `[DataType(DataType.Password)]`, display "Password"
- `Role` — `string`, default `"User"` (either `"User"` or `"Admin"`)
- `RememberMe` — `bool`, display "Remember Me"

Namespace: `spendwise.Models`

### 2. `spendwise/Models/RegisterViewModel.cs`
Properties (per spec):
- `FullName` — `[Required]`, `[StringLength(100, MinimumLength = 2)]`, display "Full Name"
- `Email` — `[Required]`, `[EmailAddress]`, display "Email Address"
- `Password` — `[Required]`, `[DataType(DataType.Password)]`, `[StringLength(100, MinimumLength = 6)]`, display "Password"
- `ConfirmPassword` — `[Required]`, `[DataType(DataType.Password)]`, `[Compare("Password", ErrorMessage = "Passwords do not match.")]`, display "Confirm Password"
- `Role` — `string`, default `"User"` (same pattern as `LoginViewModel.Role`)
- `AgreeToTerms` — `bool`, `[Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms & Conditions.")]`, display "I agree to the Terms & Conditions"

Namespace: `spendwise.Models`

### 3. `spendwise/Controllers/AccountController.cs`
New controller. Actions (all anonymous, no `[Authorize]`):

| Method | Verb | Returns |
|--------|------|---------|
| `Login()` | GET | `View()` expression-bodied |
| `Login(LoginViewModel model)` | POST + `[ValidateAntiForgeryToken]` | re-render with stub message if valid; show validation errors if invalid |
| `Register()` | GET | `View()` expression-bodied |
| `Register(RegisterViewModel model)` | POST + `[ValidateAntiForgeryToken]` | if `ModelState.IsValid` → add model error "Registration is not yet available — check back soon." and return `View(model)`; else return `View(model)` |

Namespace: `spendwise.Controllers` (file-scoped)

### 4. `spendwise/Views/Account/Login.cshtml`
(Prerequisite for step 04 — needed so the navbar Login link and Register page's "Sign In" footer link resolve correctly.)
- Navy banner strip: heading "Welcome Back" / subtext "Sign in to your SpendWise account"
- Card: white bg, `border-radius: 12px`, `max-width: 480px`, `mx-auto my-5 p-4 p-md-5`
- Card header: SpendWise logo text navy `fw-bold fs-4`
- Role toggle (User/Admin pill) — same pattern as spec 05
- Fields: Email, Password (with eye toggle)
- Validation summary: `asp-validation-summary="ModelOnly"` styled as `alert alert-danger`
- Submit: full-width gold "Sign In"
- Card footer: "Don't have an account?" + Tag Helper link to `Account/Register` (gold)
- `@section Scripts { @await Html.RenderPartialAsync("_ValidationScriptsPartial") }`

### 5. `spendwise/Views/Account/Register.cshtml`
Per spec layout requirements:
- `ViewData["Title"] = "Register";`
- Navy banner: "Create Your Account" / "Start tracking your expenses with SpendWise"
- Card: white bg, `border-radius: 12px`, `max-width: 480px`, `mx-auto my-5 p-4 p-md-5`
- Card header: SpendWise logo text navy `fw-bold fs-4`
- Role toggle (User/Admin pill) + hidden `<input asp-for="Role">`
- Form fields in order:
  1. FullName (text)
  2. Email (email)
  3. Password (password) + eye toggle + strength meter strip
  4. ConfirmPassword (password) + eye toggle
  5. AgreeToTerms checkbox — label includes Tag Helper links to `Home/TermsAndConditions` and `Home/PrivacyPolicy` (both `target="_blank"`, gold color)
- Validation summary: `asp-validation-summary="ModelOnly"` as `alert alert-danger` (stub message appears here)
- Submit: full-width gold "Create Account"
- Divider
- Card footer: "Already have an account?" + Tag Helper link to `Account/Login` (gold)
- `@section Scripts { @await Html.RenderPartialAsync("_ValidationScriptsPartial") }`

---

## Files to Modify

### 6. `spendwise/Views/Shared/_Layout.cshtml`
Current state (lines 26-27):
```html
<a href="#" class="sw-btn-outline">Login</a>
<a href="#" class="sw-btn-gold">Register</a>
```
Change to Tag Helpers:
```html
<a asp-controller="Account" asp-action="Login" class="sw-btn-outline">Login</a>
<a asp-controller="Account" asp-action="Register" class="sw-btn-gold">Register</a>
```

### 7. `spendwise/Views/Account/Login.cshtml`
(Created in step 4 above — the Register card-footer link must use Tag Helper `asp-controller="Account" asp-action="Register"` from the start, so no secondary modification needed.)

### 8. `wwwroot/js/site.js`
Add three new behaviour blocks (append, do not replace existing Chart.js code):

**A — Role toggle (shared by Login and Register pages)**
```js
document.querySelectorAll('.sw-role-toggle').forEach(toggle => {
  const buttons = toggle.querySelectorAll('.sw-role-btn');
  const hiddenInput = toggle.closest('form').querySelector('[name="Role"]');
  buttons.forEach(btn => {
    btn.addEventListener('click', () => {
      buttons.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      hiddenInput.value = btn.dataset.role;
    });
  });
});
```

**B — Password visibility toggle (shared by Login and Register)**
```js
document.querySelectorAll('.sw-pw-toggle').forEach(btn => {
  btn.addEventListener('click', () => {
    const input = btn.closest('.input-group').querySelector('input');
    const icon = btn.querySelector('i');
    if (input.type === 'password') {
      input.type = 'text';
      icon.classList.replace('bi-eye', 'bi-eye-slash');
    } else {
      input.type = 'password';
      icon.classList.replace('bi-eye-slash', 'bi-eye');
    }
  });
});
```

**C — Password strength meter (Register page only)**
Heuristic: length ≥ 6 → Weak; + uppercase → Fair; + digit → Good; + special char → Strong.
Updates Bootstrap `progress-bar` width (25/50/75/100%) and color class (`bg-danger`, `bg-warning`, `bg-info`, `bg-success`) plus a label span with the level name.
```js
const pwInput = document.getElementById('Password');
if (pwInput) {
  pwInput.addEventListener('input', () => {
    const val = pwInput.value;
    const bar = document.getElementById('sw-pw-strength-bar');
    const label = document.getElementById('sw-pw-strength-label');
    let level = 0;
    if (val.length >= 6) level = 1;
    if (level && /[A-Z]/.test(val)) level = 2;
    if (level > 1 && /[0-9]/.test(val)) level = 3;
    if (level > 2 && /[^A-Za-z0-9]/.test(val)) level = 4;
    const map = ['', 'Weak', 'Fair', 'Good', 'Strong'];
    const cls = ['', 'bg-danger', 'bg-warning', 'bg-info', 'bg-success'];
    bar.style.width = (level * 25) + '%';
    bar.className = 'progress-bar ' + (cls[level] || '');
    label.textContent = map[level] || '';
  });
}
```

### 9. `wwwroot/css/site.css`
Append auth page-specific styles:
- `.sw-auth-banner` — navy bg, white text, `padding: 2.5rem 1rem`, `text-align: center`
- `.sw-auth-card` — `max-width: 480px; margin: 2.5rem auto; border-radius: 12px; box-shadow: 0 4px 24px rgba(0,0,0,.08); background: #fff; padding: 2.5rem`
- `.sw-role-toggle` — pill-shaped segmented control, `border: 1px solid var(--sw-navy); border-radius: 50px; overflow: hidden; display: flex`
- `.sw-role-btn` — `flex: 1; border: none; background: transparent; color: var(--sw-navy); padding: .5rem 1rem; cursor: pointer; transition: background .2s`
- `.sw-role-btn.active` — `background: var(--sw-navy); color: #fff`
- `.sw-pw-toggle` — `background: transparent; border: 1px solid #dee2e6; border-left: none; cursor: pointer`

---

## Implementation Order

1. `Models/LoginViewModel.cs` — prerequisite
2. `Models/RegisterViewModel.cs`
3. `Controllers/AccountController.cs` — all four actions at once
4. `Views/Account/Login.cshtml` — prerequisite view
5. `Views/Account/Register.cshtml` — main deliverable
6. `Views/Shared/_Layout.cshtml` — update navbar links
7. `wwwroot/css/site.css` — append auth styles
8. `wwwroot/js/site.js` — append three JS blocks

---

## Verification

1. `dotnet build` — zero errors, zero warnings
2. Navigate to `/Account/Register` — page title "Register – SpendWise", navbar/footer intact
3. Click navbar "Register" button — must route to `/Account/Register` (not `#`)
4. Submit empty form — inline client-side errors appear with no page reload
5. Submit with mismatched passwords — "Passwords do not match" error inline
6. Submit without checking terms — checkbox validation error appears
7. Submit fully valid form — "Registration is not yet available" alert shown at top of card
8. Role toggle: "User" active by default; clicking "Admin" activates it; hidden `Role` field updates (verify in DevTools)
9. Eye toggle on Password and Confirm Password — work independently
10. Type into Password field — strength meter updates Weak → Fair → Good → Strong
11. Terms & Privacy links open correct pages in new tabs
12. "Sign In" footer link routes to `/Account/Login`
13. Responsive: readable at 375 px and 1440 px viewports
14. No console errors on page load
