# Ad-Blocking Repository Website

A Gatsby-based portfolio website for the Ad-Blocking Repository project, showcasing the multi-language toolkit for network protection and DNS management.

## Overview

This website provides an overview of the Ad-Blocking Repository, including:
- **Rules Compilers**: Four different language implementations (TypeScript, .NET, Python, Rust)
- **AdGuard API Client**: Complete C# SDK with interactive console UI
- **Automation Scripts**: PowerShell and shell scripts for cross-platform automation
- **Docker Environment**: Pre-configured development container

## Development

### Prerequisites

- Node.js 18+
- npm or yarn

### Installation

```bash
npm install
```

### Running Locally

```bash
# Development server (hot reload)
npm run develop

# Open http://localhost:8000 in your browser
```

### Building for Production

```bash
# Build static site
npm run build

# Serve the built site locally
npm run serve
```

### Cleaning Cache

```bash
npm run clean
```

## Content Management

Content is managed through JSON and Markdown files in the `content/` directory:

- `content/settings.json` - Site metadata and navigation
- `content/sections/hero/hero.json` - Hero section
- `content/sections/about/about.md` - About section
- `content/sections/projects/projects.json` - Projects showcase
- `content/sections/interests/interests.json` - Features/interests
- `content/sections/contact/contact.json` - Contact information

## Deployment

The website is automatically deployed to GitHub Pages via the GitHub Actions workflow `.github/workflows/gatsby.yml` on pushes to the main branch.

**Live Site**: https://jaypatrick.github.io/ad-blocking/

## Technology

- **Gatsby 5**: React-based static site generator
- **gatsby-theme-portfolio-minimal**: Portfolio theme for Gatsby
- **React 18**: UI library

## License

See the main repository LICENSE file.
