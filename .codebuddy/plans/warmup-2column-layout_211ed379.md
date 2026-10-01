---
name: warmup-2column-layout
overview: "Restyle Warmup.razor and Warmup.razor.css to a 2-column layout: left column = info card + play button, right column = timer (drum picker + running controls). CSS-only changes, no feature/logic changes."
todos:
  - id: restructure-markup
    content: "Restructure Warmup.razor DOM: move play button and running controls into left column under info card"
    status: completed
  - id: fix-and-add-css
    content: Fix workspace class name and add all missing timer/drum-picker styles with cyan theme to Warmup.razor.css
    status: completed
    dependencies:
      - restructure-markup
---

## Product Overview
Create a new Blazor page at `/op/about` that replicates the Esco Lifesciences "Contact Us" reference image, matching its layout, typography, colors, and visual elements.

## Core Features
- Custom top header with Esco brand badge (left), "Contact Us" title banner (center), and date/time badge (right)
- World-map background with location markers
- Centered "We'd love to hear from you!" heading and subtitle
- Contact details card for Esco Lifesciences Group Ltd. with address, phone, fax, email, website and small icons
- Light-blue/cyan color palette matching the reference photo

## Tech Stack
- Blazor Server (.NET 8) with scoped CSS isolation (`About.razor.css`)
- Route: `@page "/op/about"` in `Components/Op/About.razor`
- Reuse existing `EscoBrand` component for the left header badge
- Inline SVG for the world-map background and contact icons (no new static assets required)

## Implementation Approach
Create `About.razor` as a read-only contact screen wrapped in the existing `hmi-shell` class. Build a custom three-part header inline (EscoBrand + centered title banner + right date/time badge) instead of using `InnerTopBar`, because the reference image has a distinct dashboard-style header. Use an inline SVG world map as the main background with subtle continent shapes and red location pins. Style all typography, shadows, gradients, and spacing in the scoped CSS file to match the image. Expose `CurrentTime` as a `[Parameter]` so the date/time badge matches the reference by default but can be overridden.

## Implementation Notes
- The existing `EscoBrand.razor` already renders the exact "ESCO / CLASS II A2 / AC2" badge seen in the image — reuse it unchanged.
- No world-map image asset exists, so an inline SVG is the cleanest zero-asset solution.
- No contact icons exist in `Components/Icons/`, so use small inline SVGs for building, phone, fax, email, and website.
- Keep the page non-interactive (no buttons, no navigation callbacks) because the reference image is a static contact screen.

## Design Style
Clean, corporate HMI contact screen with a soft cyan/blue palette. The background uses a faint world-map watermark so the text remains highly readable. The header is a dark blue dashboard-style bar with the Esco badge on the left, a centered gradient "Contact Us" banner, and a compact date/time badge on the right. Typography is clear and sans-serif, with a bold dark-blue heading and smaller contact details.

## Page Layout
Single full-screen view inside `hmi-shell`:
1. **Top header bar** — three-column flex: brand badge | title banner | date/time badge
2. **Main viewport** — centered content block over a world-map watermark:
   - Heading "We'd love to hear from you!"
   - Subheading paragraph
   - Left-aligned contact card with icon + text rows
