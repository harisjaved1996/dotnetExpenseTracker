# Implementation Plan: Design Privacy Policy Page

## Context

The footer "Privacy Policy" anchor in `_Layout.cshtml` currently points to `href="#"` with no real route or view. The Terms & Conditions page also has a placeholder `href="#"` link in its "Data and Privacy" section. This plan wires up a proper GET route, creates the Privacy Policy Razor view, and updates both placeholder links to use Tag Helper URLs. The page is fully public, static content — no DB calls needed.

---

## Files to Modify

| File | Change |
|---|---|
| `spendwise/Controllers/HomeController.cs` | Add `PrivacyPolicy()` expression-bodied action |
| `spendwise/Views/Shared/_Layout.cshtml` | Replace `href="#"` on footer "Privacy Policy" link with Tag Helper |
| `spendwise/Views/Home/TermsAndConditions.cshtml` | Replace `href="#"` in "Data and Privacy" section with Tag Helper |
| `spendwise/wwwroot/css/site.css` | Add `.sw-section-heading` class (does not exist yet) |

## Files to Create

| File | Purpose |
|---|---|
| `spendwise/Views/Home/PrivacyPolicy.cshtml` | Full Privacy Policy page with 9 content sections |

---

## Step-by-Step Implementation

### Step 1 — Add `PrivacyPolicy()` action to `HomeController.cs`

Add an expression-bodied action immediately after the existing `TermsAndConditions()` action (line ~14). No `[Authorize]`, no DB calls.

```csharp
public IActionResult PrivacyPolicy() => View();
```

> **Note:** The existing `Privacy()` action (scaffolded default) stays unchanged — it is a separate route and view.

---

### Step 2 — Add `.sw-section-heading` to `site.css`

The class does not exist yet. Add it under the existing section heading styles (after the Terms & Conditions banner block). The spec requires a 4px gold left-border accent with 0.75rem left padding.

```css
/* Privacy Policy */
.sw-section-heading {
    border-left: 4px solid var(--sw-gold);
    padding-left: 0.75rem;
}
```

---

### Step 3 — Update footer link in `_Layout.cshtml`

**Current (line ~45):**
```html
<a href="#" class="sw-footer-link">Privacy Policy</a>
```

**Replace with:**
```html
<a asp-controller="Home" asp-action="PrivacyPolicy" class="sw-footer-link">Privacy Policy</a>
```

---

### Step 4 — Update Terms & Conditions link in `TermsAndConditions.cshtml`

Locate the "Data and Privacy" section (lines ~41-47). Find the `<a href="#">Privacy Policy</a>` anchor.

**Replace with:**
```html
<a asp-controller="Home" asp-action="PrivacyPolicy">Privacy Policy</a>
```

---

### Step 5 — Create `Views/Home/PrivacyPolicy.cshtml`

Full page structure following the navy/gold design language. Reuse existing CSS classes:
- `.sw-hero` / `.sw-hero h1` / `.sw-hero p` — top banner (same pattern as Terms & Conditions page)
- `.sw-btn-gold` — "Back to Home" primary button
- `.sw-btn-outline` — "View Terms & Conditions" secondary button (**Note:** the spec mentions `sw-btn-gold-outline` but `site.css` only defines `.sw-btn-outline` — use `.sw-btn-outline`, which is the existing equivalent)
- `.sw-section-heading` — added in Step 2

**Page skeleton:**

```cshtml
@{
    ViewData["Title"] = "Privacy Policy";
}

<!-- Top Banner -->
<div class="sw-hero">
    <h1>Privacy Policy</h1>
    <p>Effective Date: January 1, 2026</p>
</div>

<!-- Content -->
<div class="container py-5">
    <div class="row">
        <div class="col-lg-9 mx-auto">

            <!-- Section 1: Introduction -->
            <h2 class="sw-section-heading mb-3">Introduction</h2>
            <p>...</p>

            <!-- Section 2: Information We Collect -->
            <h2 class="sw-section-heading mb-3 mt-5">Information We Collect</h2>
            <p>...</p>

            <!-- Section 3: How We Use Your Information -->
            <!-- Section 4: Data Storage and Security -->
            <!-- Section 5: Sharing of Information -->
            <!-- Section 6: Cookies and Tracking -->
            <!-- Section 7: Your Rights -->
            <!-- Section 8: Changes to This Policy -->
            <!-- Section 9: Contact Us -->

            <!-- Bottom Buttons -->
            <div class="d-flex gap-3 mt-5">
                <a asp-controller="Home" asp-action="Index" class="sw-btn-gold">Back to Home</a>
                <a asp-controller="Home" asp-action="TermsAndConditions" class="sw-btn-outline">View Terms &amp; Conditions</a>
            </div>

        </div>
    </div>
</div>
```

All nine sections use realistic placeholder copy as specified in the spec.

---

## CSS Class Inventory (No Changes Needed Except Step 2)

| Class | Status | Used For |
|---|---|---|
| `.sw-hero` | Exists | Top banner navy background |
| `.sw-btn-gold` | Exists | "Back to Home" button |
| `.sw-btn-outline` | Exists | "View Terms & Conditions" button |
| `.sw-section-heading` | **Add in Step 2** | Section `<h2>` gold left-border accent |

---

## Verification

1. Run `dotnet build` — expect zero errors and zero warnings.
2. Navigate to `/Home/PrivacyPolicy` — page title should read "Privacy Policy – SpendWise".
3. Click "Privacy Policy" in the site footer — must navigate to the Privacy Policy page (not `#`).
4. Open Terms & Conditions page → "Data and Privacy" section → click "Privacy Policy" link — must navigate to Privacy Policy page (not `#`).
5. Verify all nine content sections are visible on the page.
6. Click "Back to Home" — navigates to landing page.
7. Click "View Terms & Conditions" — navigates to Terms & Conditions page.
8. Resize to 375 px and 1440 px — page must be readable at both widths.
9. Open browser DevTools Console — no errors on page load.
