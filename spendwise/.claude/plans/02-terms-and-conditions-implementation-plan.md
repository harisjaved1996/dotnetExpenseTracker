# Implementation Plan: Terms & Conditions Page (Spec 02)

## Context

The SpendWise footer has a "Terms & Conditions" link that currently points to `href="#"` with no real route. This plan wires up a proper MVC route, creates a styled Razor view with eight legal sections, and updates the footer anchor to use ASP.NET Core Tag Helpers. No database changes are required — the page is static content.

---

## Files to Modify

### 1. `spendwise/Controllers/HomeController.cs`

**Change:** Add one expression-bodied GET action after the existing `Privacy()` action.

```csharp
public IActionResult TermsAndConditions() => View();
```

No `[Authorize]`, no async, no DB calls — matches the spec rule exactly.

---

### 2. `spendwise/Views/Shared/_Layout.cshtml`

**Change:** Line 43 — replace the placeholder anchor with Tag Helpers.

**Before:**
```html
<a href="#" class="sw-footer-link">Terms &amp; Conditions</a>
```

**After:**
```html
<a asp-controller="Home" asp-action="TermsAndConditions" class="sw-footer-link">Terms &amp; Conditions</a>
```

---

### 3. `spendwise/wwwroot/css/site.css`

**Change:** Append two new rule blocks at the end of the file.

```css
/* ── Terms & Conditions page ── */
.sw-terms-banner {
  background-color: var(--sw-navy);
  color: #fff;
  padding: 72px 0 56px;
  text-align: center;
}

.sw-terms-banner h1 {
  font-size: 2.25rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.sw-terms-banner p {
  opacity: 0.75;
  font-size: 0.95rem;
  margin-bottom: 0;
}

.sw-terms-heading {
  border-left: 4px solid var(--sw-gold);
  padding-left: 0.75rem;
  margin-top: 2.5rem;
  margin-bottom: 1rem;
}
```

These two classes keep the Terms page consistent with the navy/gold design language without touching existing rules.

---

## Files to Create

### 4. `spendwise/Views/Home/TermsAndConditions.cshtml`

Full Razor view structure:

```razor
@{
    ViewData["Title"] = "Terms & Conditions";
}

<!-- Top banner -->
<section class="sw-terms-banner">
    <div class="container">
        <h1>Terms &amp; Conditions</h1>
        <p>Effective Date: January 1, 2026</p>
    </div>
</section>

<!-- Content -->
<section class="bg-white py-5">
    <div class="container">
        <div class="col-lg-9 mx-auto">

            <h2 class="sw-terms-heading">1. Acceptance of Terms</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">2. Use of the Service</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">3. User Accounts</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">4. Data and Privacy</h2>
            <p>... <a href="#">Privacy Policy</a> ...</p>

            <h2 class="sw-terms-heading">5. Intellectual Property</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">6. Limitation of Liability</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">7. Changes to Terms</h2>
            <p>...</p>

            <h2 class="sw-terms-heading">8. Contact Us</h2>
            <p>... legal@spendwise.com ...</p>

            <div class="mt-5">
                <a asp-controller="Home" asp-action="Index" class="sw-btn-gold">Back to Home</a>
            </div>

        </div>
    </div>
</section>
```

**Reuses:** `sw-btn-gold` (already defined in `wwwroot/css/site.css` lines 62–78), `--sw-navy`/`--sw-gold` custom properties (lines 2–7).

---

## Implementation Order

1. Add CSS rules to `site.css` (no risk, purely additive)
2. Add `TermsAndConditions()` action to `HomeController.cs`
3. Create `Views/Home/TermsAndConditions.cshtml` with all eight sections
4. Update footer anchor in `_Layout.cshtml`

---

## Verification

```bash
dotnet build --project spendwise   # must produce 0 errors, 0 warnings
dotnet run --project spendwise     # start dev server
```

Then verify:
- `/Home/TermsAndConditions` loads with title "Terms & Conditions – SpendWise"
- Navbar and footer render identically to other pages
- Clicking "Terms & Conditions" in the footer navigates to the page (no longer `#`)
- All 8 section headings visible with gold left-border accent
- "Back to Home" button navigates to `/`
- No browser console errors
- Readable on 375 px and 1440 px viewports (Bootstrap `col-lg-9 mx-auto` handles this)
