# Linear Documentation Import Tool

Import ad-blocking repository documentation into Linear project management system.

## Overview

This tool parses the repository's Linear documentation (`docs/LINEAR_DOCUMENTATION.md`) and imports it into Linear, creating:

- A project to track the ad-blocking system
- Issues for each roadmap item
- Documentation issues for each component
- A main documentation issue with the full content

## Prerequisites

- Node.js 20+
- npm or yarn
- Linear account with API access

## Installation

```bash
cd src/linear
npm install
npm run build
```

## Configuration

1. Copy the environment example file:
   ```bash
   cp .env.example .env
   ```

2. Get your Linear API key:
   - Go to **Linear Settings** > **API** > **Personal API keys**
   - Create a new key with appropriate permissions
   - Copy the key to your `.env` file

3. Edit `.env` with your configuration:
   ```env
   LINEAR_API_KEY=lin_api_your_key_here
   LINEAR_TEAM_ID=optional_team_id
   LINEAR_PROJECT_NAME=Ad-Blocking Documentation
   ```

## Usage

### Import Documentation

```bash
# Full import with default settings
npm run import:docs

# Dry run (preview without making changes)
npm run import:dry-run

# Custom file path
npm run import -- --file /path/to/documentation.md

# Specify project name
npm run import -- --project "My Project Name"

# Use specific team
npm run import -- --team team_id_here
```

### List Available Resources

```bash
# List available teams
npm run import -- --list-teams

# List existing projects
npm run import -- --list-projects
```

### Command Options

| Option | Description |
|--------|-------------|
| `-f, --file <path>` | Path to markdown documentation file |
| `-t, --team <id>` | Linear team ID (defaults to first team) |
| `-p, --project <name>` | Linear project name |
| `--dry-run` | Preview import without making changes |
| `--no-project` | Skip project creation |
| `--no-issues` | Skip issue creation |
| `--no-docs` | Skip documentation issue creation |
| `--list-teams` | List available teams and exit |
| `--list-projects` | List existing projects and exit |
| `-v, --verbose` | Verbose output |

## What Gets Imported

### Roadmap Items

Roadmap items (checkbox lists in the documentation) are converted to Linear issues:

```markdown
- [ ] Additional filter sources integration
- [ ] Real-time filter update notifications
```

Becomes:
- Issue: "Additional filter sources integration"
- Issue: "Real-time filter update notifications"

### Components

Each documented component creates a detailed documentation issue:

- Filter Rules (`/rules/`)
- Rules Compiler TypeScript (`/src/rules-compiler-typescript/`)
- API Client (`/src/adguard-api-dotnet/`)
- etc.

### Main Documentation

The full documentation content is imported as a comprehensive reference issue.

## Project Structure

```
src/linear/
├── src/
│   ├── types.ts          # TypeScript interfaces
│   ├── parser.ts         # Markdown parsing logic
│   ├── linear-client.ts  # Linear API wrapper
│   └── linear-import.ts  # Main entry point
├── .env.example          # Environment configuration template
├── package.json          # Dependencies and scripts
├── tsconfig.json         # TypeScript configuration
└── README.md             # This file
```

## Development

### Build

```bash
npm run build
```

### Run directly

```bash
node dist/linear-import.js --help
```

## Troubleshooting

### "LINEAR_API_KEY environment variable is required"

Ensure you've created a `.env` file with your API key:
```bash
cp .env.example .env
# Edit .env and add your API key
```

### "No teams found in Linear workspace"

Make sure your API key has access to at least one team in your Linear workspace.

### "Failed to create project"

Check that your API key has permissions to create projects. You may need to use the `--team` option to specify a valid team ID.

### Rate Limiting

Linear has API rate limits. If you encounter rate limiting issues, wait a few minutes and try again.

## License

GPL-3.0 - See [LICENSE](../../LICENSE) for details.
