# Implementation Plan: Design Home / Landing Page

## Context
The project currently shows the default ASP.NET scaffold placeholder at `/`. The goal is to replace it with a polished SpendWise marketing/landing page that communicates the app's purpose, introduces features, and guides visitors toward Login or Register. No database or controller logic is involved — this is a pure UI change.

---

## Files to Change

| File | Change Type | Summary |
|------|-------------|---------|
| `Views/Shared/_Layout.cshtml` | Modify | Fixed navy navbar, sticky footer, remove container wrapper |
| `Views/Home/Index.cshtml` | Full replace | Landing page: Hero + Features + How It Works |
| `wwwroot/css/site.css` | Modify | Design tokens, navbar/footer/hero/section styles |

No new files. No controller changes. No migrations.

---

## Step 1 — `Views/Shared/_Layout.cshtml`

### Navbar changes
- Replace `navbar-light bg-white border-bottom` with `navbar-dark` + inline `style="background-color: var(--sw-navy)"` or a `.sw-navbar` CSS class
- Add `fixed-top` Bootstrap class so the bar stays at the top on scroll
- Change brand text from `spendwise` → `SpendWise`, apply gold color via CSS class `.sw-brand`
- Brand `<a>` uses `asp-controller="Home" asp-action="Index"` (Tag Helper — no `asp-area=""` needed)
- Change `navbar-expand-sm` → `navbar-expand-md` (better breakpoint for the auth buttons)
- Change `data-bs-target=".navbar-collapse"` → `data-bs-target="#navbarMain"` and give the collapse div `id="navbarMain"` (Bootstrap 5 best practice)
- Remove the left `<ul>` nav links (Home, Privacy) — not needed on a public landing page
- Add right-side auth links inside the collapse div, aligned with `ms-auto`:
  - `<a href="#" class="sw-btn-outline">Login</a>`
  - `<a href="#" class="sw-btn-gold">Register</a>`

### Body / main
- Remove `<div class="container">` wrapper around `@RenderBody()` — each view will control its own width
- Keep `<main role="main">@RenderBody()</main>` unwrapped

### Footer changes
- Remove old `border-top footer text-muted` classes
- Apply `.sw-footer` class (navy bg, white text, sticky-bottom via flexbox on body)
- Content: copyright left, pipe-separated links right
  - `© 2026 SpendWise. All rights reserved.`
  - `<a href="#">Terms &amp; Conditions</a> | <a href="#">Privacy Policy</a>` — gold color

### CDN additions (in `<head>`)
- Bootstrap Icons CDN: `https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css`
  - Bootstrap JS is already local (`~/lib/bootstrap/dist/js/bootstrap.bundle.min.js`) — no change needed

---

## Step 2 — `Views/Home/Index.cshtml`

Three full-width sections. No surrounding container — each section manages its own padding.

### Title
```
@{ ViewData["Title"] = "Home – SpendWise"; }
```

### Section 1 — Hero
- `<section class="sw-hero">` — full-width, navy gradient background
- Centered content inside `<div class="container">`
- `<h1>` — "Take Control of Your Spending" — white, large (display-4 or display-5)
- `<p>` — subheadline — white/semi-transparent, max-width ~600 px, centered
- Two buttons:
  - `<a href="#" class="sw-btn-gold">Get Started</a>`
  - `<a href="#features" class="sw-btn-outline-light">Learn More</a>` — scrolls to features section

### Section 2 — Features
- `<section id="features" class="sw-features">` — light gray bg (`#f7f8fa`)
- `<div class="container">` with section heading "Why SpendWise?"
- Bootstrap `row row-cols-1 row-cols-md-3 g-4` grid for 3 cards
- Each card (`<div class="sw-feature-card">`) contains:
  - Bootstrap Icon `<i class="bi bi-...">` in gold (Track: `bi-cash-stack`, Categories: `bi-tags`, Insights: `bi-bar-chart-line`)
  - Card title + 1–2 sentence description

### Section 3 — How It Works
- `<section class="sw-steps">` — white background
- Section heading "Get Started in 3 Simple Steps"
- Bootstrap `row row-cols-1 row-cols-md-3 g-4` for 3 step cards
- Each step: large gold step number, bold title, one-line description
  1. Create Account
  2. Add Expenses
  3. See Your Insights

---

## Step 3 — `wwwroot/css/site.css`

### Design tokens (add to `:root`)
```css
:root {
  --sw-navy: #1e3a5f;
  --sw-gold: #f5a623;
  --sw-gold-hover: #e09517;
  --sw-gray-bg: #f7f8fa;
}
```

### Layout (replace conflicting body rule)
```css
body {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  margin: 0;
  padding-top: 64px;   /* offset for fixed navbar */
  font-family: inherit;
}

main {
  flex: 1;
}
```

### Navbar
```css
.sw-navbar { background-color: var(--sw-navy); }
.sw-brand  { color: var(--sw-gold) !important; font-weight: 700; font-size: 1.25rem; }
```

### Buttons
```css
.sw-btn-gold {
  background-color: var(--sw-gold); color: #fff; border: 2px solid var(--sw-gold);
  padding: 8px 22px; border-radius: 8px; font-weight: 500; text-decoration: none; display: inline-block;
}
.sw-btn-gold:hover { background-color: var(--sw-gold-hover); border-color: var(--sw-gold-hover); color: #fff; }

.sw-btn-outline {
  background: transparent; color: var(--sw-gold); border: 2px solid var(--sw-gold);
  padding: 8px 22px; border-radius: 8px; font-weight: 500; text-decoration: none; display: inline-block;
}
.sw-btn-outline:hover { background-color: var(--sw-gold); color: #fff; }

.sw-btn-outline-light {
  background: transparent; color: #fff; border: 2px solid #fff;
  padding: 10px 28px; border-radius: 8px; font-weight: 500; text-decoration: none; display: inline-block;
}
.sw-btn-outline-light:hover { background-color: rgba(255,255,255,0.15); color: #fff; }
```

### Hero section
```css
.sw-hero {
  background: linear-gradient(135deg, var(--sw-navy) 0%, #2a4f7c 100%);
  padding: 100px 0 80px;
  text-align: center;
  color: #fff;
}
.sw-hero h1 { font-weight: 700; margin-bottom: 1rem; }
.sw-hero p  { max-width: 600px; margin: 0 auto 2rem; opacity: 0.9; font-size: 1.1rem; }
.sw-hero .sw-hero-btns { display: flex; gap: 16px; justify-content: center; flex-wrap: wrap; }
```

### Features section
```css
.sw-features { background-color: var(--sw-gray-bg); padding: 80px 0; }
.sw-features h2 { text-align: center; font-weight: 700; color: var(--sw-navy); margin-bottom: 48px; }

.sw-feature-card {
  background: #fff; border: 1px solid #e5e7eb; border-radius: 12px;
  padding: 32px 24px; text-align: center;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  transition: box-shadow 0.2s;
}
.sw-feature-card:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.sw-feature-card .sw-icon { font-size: 2.5rem; color: var(--sw-gold); margin-bottom: 16px; display: block; }
.sw-feature-card h4 { font-weight: 600; color: var(--sw-navy); margin-bottom: 8px; }
.sw-feature-card p { color: #6b7280; font-size: 0.95rem; margin: 0; }
```

### How It Works section
```css
.sw-steps { background: #fff; padding: 80px 0; }
.sw-steps h2 { text-align: center; font-weight: 700; color: var(--sw-navy); margin-bottom: 48px; }

.sw-step-card { text-align: center; padding: 24px; }
.sw-step-number {
  width: 56px; height: 56px; border-radius: 50%;
  background-color: var(--sw-gold); color: #fff;
  font-size: 1.5rem; font-weight: 700;
  display: flex; align-items: center; justify-content: center;
  margin: 0 auto 16px;
}
.sw-step-card h4 { font-weight: 600; color: var(--sw-navy); margin-bottom: 8px; }
.sw-step-card p  { color: #6b7280; font-size: 0.95rem; margin: 0; }
```

### Footer
```css
.sw-footer {
  background-color: var(--sw-navy); color: #fff;
  padding: 20px 0; font-size: 0.875rem;
}
.sw-footer-link { color: var(--sw-gold); text-decoration: none; }
.sw-footer-link:hover { color: var(--sw-gold-hover); text-decoration: underline; }
.sw-footer-sep { color: rgba(255,255,255,0.4); margin: 0 10px; }
```

---

## Verification Steps

1. Run `dotnet build --project spendwise` — must produce **0 errors, 0 warnings**
2. Run `dotnet run --project spendwise` — open `https://localhost:5108/`
3. Check:
   - [ ] Page title is "Home – SpendWise"
   - [ ] Navbar is navy and stays fixed on scroll
   - [ ] "SpendWise" brand text is gold and navigates home on click
   - [ ] "Login" and "Register" buttons appear on the right; clicking them does nothing
   - [ ] Hamburger toggle works on a narrow viewport (≤ 767 px)
   - [ ] Hero section shows headline, subheadline, and two CTA buttons
   - [ ] Features section shows 3 cards in a row (desktop) / stacked (mobile)
   - [ ] How It Works section shows 3 numbered steps
   - [ ] Footer shows copyright + Terms & Conditions + Privacy Policy; links do nothing
   - [ ] No browser console errors
