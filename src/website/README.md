# Ad-Blocking Toolkit Website

This is a Gatsby-powered documentation website for the Ad-Blocking repository.

## Development

```bash
# Install dependencies
npm install

# Start development server
npm run develop

# Build for production
npm run build

# Serve production build
npm run serve
```

## Structure

- `src/pages/` - Static pages (home, getting started, etc.)
- `src/templates/` - Templates for dynamically generated pages
- `src/components/` - Reusable React components
  - `Layout.js` - Main layout with header, nav, footer, search, and theme toggle
  - `Search.js` - Client-side search component with autocomplete
  - `ThemeToggle.js` - Dark/light mode toggle with persistence
- `src/styles/` - Global CSS styles with dark mode support
- `gatsby-config.js` - Gatsby configuration
- `gatsby-node.js` - Node APIs for page generation

## Features

- 📚 Automatic documentation from markdown files
- 🔍 Organized by category (Guides, API, Technical)
- 📱 Responsive design
- 🚀 Fast static site generation
- 🎨 Clean, accessible UI
- 🌙 **Dark mode** with theme toggle and localStorage persistence
- 🔎 **Client-side search** across all documentation
- 🔗 Verified internal links (no 404 errors)

## Deployment

The site is automatically built and deployed to GitHub Pages when changes are pushed to the main branch.

Site URL: https://jaypatrick.github.io/ad-blocking

## New Features

### Dark Mode
- Toggle between light and dark themes using the button in the header (moon/sun icon)
- Theme preference is saved in localStorage and persists across sessions
- Smooth transitions between themes
- All components and pages support both themes

### Search
- Real-time client-side search across all documentation
- Search as you type with instant results
- Click any result to navigate to that page
- Clear button to reset search
- Keyboard-friendly (Enter to navigate to first result)

### Link Verification
All internal links have been verified and fixed:
- Improvements page now uses correct document slugs (UPPERCASE_SNAKE_CASE format)
- All navigation links tested and working
- No 404 errors on internal pages
