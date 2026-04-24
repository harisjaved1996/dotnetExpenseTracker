# Spec: Design Home / Landing Page

## Overview
Replace the default ASP.NET scaffold placeholder with a polished SpendWise landing page. This is the first thing any visitor sees when they arrive at the site. It should communicate the app's purpose, introduce key features, and guide the visitor toward signing up or logging in.

## Depends on
None — this is step 01 and has no prerequisites.

## Routes (Controller Actions)
- `HomeController` → `Index` — GET — Returns the landing page.

## Database changes
No changes

## Models & ViewModels
No changes — no data is fetched for this page.

## Views
- **Modify:** `Views/Home/Index.cshtml` — replace the scaffold placeholder with the SpendWise landing page (hero section, features section, how-it-works section)
- **Modify:** `Views/Shared/_Layout.cshtml` — apply SpendWise brand styles: fixed navy navbar (`#1e3a5f`), gold accent (`#f5a623`), "SpendWise" brand name on the left, Login/Register links on the right, sticky footer with Terms & Conditions and Privacy Policy links

## Files to change
- `Views/Home/Index.cshtml` — full replacement with landing page UI
- `Views/Shared/_Layout.cshtml` — fixed navbar + footer updates
- `wwwroot/css/site.css` — add SpendWise design tokens (navy/gold CSS custom properties) and component styles

## Files to create
None

## New NuGet packages
None

## Rules for implementation
- Follow ASP.NET Core MVC conventions — no Razor Pages, no Web API controllers
- File-scoped namespaces matching folder path (`namespace spendwise.Controllers;`)
- Bootstrap 5 utility classes preferred; custom CSS only when Bootstrap cannot achieve the design
- All custom styles go in `wwwroot/css/site.css`; all custom scripts in `wwwroot/js/site.js`
- All views must extend `_Layout.cshtml`
- No npm, Webpack, TypeScript, or JS build tooling
- Design language: navy `#1e3a5f` as primary brand color, gold `#f5a623` as accent; define as CSS custom properties on `:root`
- Do not add `[Authorize]` or any auth gates; the page is fully public
- Remove the old container wrapper around `@RenderBody()` in `_Layout.cshtml` — let each view manage its own width so the landing page can use full-width sections
- The navbar must use `navbar-dark` (not `navbar-light`) because it sits on a navy background

## Navbar requirements
- `position: fixed; top: 0` — sticks to the top of the viewport at all times
- **Left side:** "SpendWise" brand name in gold (`#f5a623`), links to `HomeController → Index` via Tag Helpers
- **Right side:** "Login" anchor tag and "Register" anchor tag, both `href="#"` (no real route yet), styled as gold-outlined and gold-filled buttons respectively
- Hamburger toggle collapses the nav on mobile viewports and must work correctly
- `body` must have `padding-top` equal to the navbar height (~64 px) so content is not hidden behind the fixed bar

## Footer requirements
- Sticky at the bottom of the page — appears below all content, never overlaps (use flexbox layout: `body` as flex column, `min-height: 100vh`, `main` grows with `flex: 1`)
- Navy background (`#1e3a5f`), white text
- **Left/center:** copyright text — `© 2026 SpendWise. All rights reserved.`
- **Right/center:** two links — "Terms & Conditions" and "Privacy Policy", both `href="#"` (no real route), gold color (`#f5a623`), separated by a `|` pipe character
- No real navigation or routing behind these links at this stage

## Landing page sections (Index.cshtml)

### 1. Hero Section
- Full-width, navy background with a subtle gradient
- Large headline: **"Take Control of Your Spending"**
- Subheadline: *"SpendWise helps you track every expense, understand your spending habits, and reach your financial goals — all in one place."*
- Two CTA buttons: "Get Started" (gold filled, `href="#"`) and "Learn More" (gold outline, scrolls to the features section via anchor link)
- Centered content, white text, responsive padding

### 2. Features Section
- Light gray background (`#f7f8fa`), section heading: **"Why SpendWise?"**
- Three cards in a row on ≥ 768 px, stacked vertically on smaller viewports
- Card 1 — **Track Expenses:** icon + short description
- Card 2 — **Smart Categories:** icon + short description
- Card 3 — **Financial Insights:** icon + short description
- White cards with subtle border and shadow, gold icon accent

### 3. How It Works Section
- White background, section heading: **"Get Started in 3 Simple Steps"**
- Three numbered steps in a row on ≥ 768 px, stacked on mobile
- Step 1 — **Create Account**
- Step 2 — **Add Expenses**
- Step 3 — **See Your Insights**
- Each step has a large gold step number, a short title, and a one-line description

## Definition of done
- [ ] `dotnet build` produces zero errors and zero warnings
- [ ] Navigating to `https://localhost:5108` loads a page titled "Home – SpendWise"
- [ ] The navbar is fixed to the top; scrolling the page does not cause content to hide behind it
- [ ] The navbar shows "SpendWise" in gold on the left; "Login" and "Register" links appear on the right
- [ ] Clicking "Login" or "Register" does nothing (href="#")
- [ ] The hamburger toggle shows on mobile and correctly collapses/expands the nav
- [ ] The hero section is visible with the headline, subheadline, and two CTA buttons
- [ ] The features section shows three cards; they stack on a 375 px viewport and sit in a row on ≥ 768 px
- [ ] The "How It Works" section shows three numbered steps
- [ ] The footer is visible at the bottom of the page with "Terms & Conditions" and "Privacy Policy" links
- [ ] Clicking either footer link does nothing (href="#")
- [ ] No browser console errors on page load
