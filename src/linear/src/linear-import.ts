#!/usr/bin/env -S deno run --allow-read --allow-write --allow-env --allow-net
/**
 * Linear Documentation Import Tool
 *
 * Import ad-blocking repository documentation into Linear project management.
 * Deno-only implementation.
 *
 * Usage:
 *   deno task import:docs          # Import documentation with default settings
 *   deno task import:dry-run       # Preview what would be imported
 *
 * Environment Variables:
 *   ADGUARD_LINEAR_API_KEY  - Your Linear API key (required, recommended)
 *   LINEAR_API_KEY          - Legacy format (backward compatibility)
 *   ADGUARD_LINEAR_TEAM_ID  - Team ID to use (optional, defaults to first team)
 *   LINEAR_TEAM_ID          - Legacy format (backward compatibility)
 *   ADGUARD_LINEAR_PROJECT_NAME - Project name (optional, defaults to document title)
 *   LINEAR_PROJECT_NAME     - Legacy format (backward compatibility)
 */

import { Command } from 'commander';
import { config } from 'dotenv';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { existsSync } from 'node:fs';

import { LinearImporter } from './linear-client.ts';
import {
  extractComponents,
  extractRoadmapItems,
  flattenSections,
  parseMarkdownFile,
} from './parser.ts';
import type { ImportConfig } from './types.ts';

// Load environment variables
const __dirname = dirname(fileURLToPath(import.meta.url));
config({ path: resolve(__dirname, '../.env') });

const DEFAULT_DOC_PATH = resolve(__dirname, '../../../docs/LINEAR_DOCUMENTATION.md');

async function main(): Promise<void> {
  const program = new Command();

  program
    .name('linear-import')
    .description('Import documentation into Linear project management')
    .version('1.0.0')
    .option('-f, --file <path>', 'Path to markdown documentation file', DEFAULT_DOC_PATH)
    .option('-t, --team <id>', 'Linear team ID (defaults to first team)')
    .option('-p, --project <name>', 'Linear project name')
    .option('--dry-run', 'Preview import without making changes', false)
    .option('--no-project', 'Skip project creation')
    .option('--no-issues', 'Skip issue creation')
    .option('--no-docs', 'Skip documentation issue creation')
    .option('--list-teams', 'List available teams and exit')
    .option('--list-projects', 'List existing projects and exit')
    .option('-v, --verbose', 'Verbose output')
    .parse(Deno.args);

  const options = program.opts();

  // Check for API key (try new format first, then fall back to legacy)
  const apiKey = Deno.env.get('ADGUARD_LINEAR_API_KEY') || Deno.env.get('LINEAR_API_KEY');
  if (!apiKey) {
    console.error('Error: ADGUARD_LINEAR_API_KEY environment variable is required');
    console.error('(Legacy LINEAR_API_KEY is also supported for backward compatibility)');
    console.error('\nTo get your API key:');
    console.error('1. Go to Linear Settings > API');
    console.error('2. Create a new personal API key');
    console.error('3. Set it in your .env file or environment');
    Deno.exit(1);
  }

  const importConfig: ImportConfig = {
    teamId: options['team'] || Deno.env.get('ADGUARD_LINEAR_TEAM_ID') || Deno.env.get('LINEAR_TEAM_ID') || '',
    projectName: options['project'] || Deno.env.get('ADGUARD_LINEAR_PROJECT_NAME') || Deno.env.get('LINEAR_PROJECT_NAME') || '',
    createProject: options['project'] !== false,
    createIssues: options['issues'] !== false,
    createDocuments: options['docs'] !== false,
    dryRun: options['dryRun'],
  };

  try {
    const importer = new LinearImporter(apiKey, importConfig);
    await importer.initialize();

    // Handle list commands
    if (options['listTeams']) {
      console.log('\nAvailable Teams:');
      const teams = await importer.listTeams();
      for (const team of teams) {
        console.log(`  ${team.name} (${team.id})`);
      }
      return;
    }

    if (options['listProjects']) {
      console.log('\nExisting Projects:');
      const projects = await importer.listProjects();
      if (projects.length === 0) {
        console.log('  No projects found');
      } else {
        for (const project of projects) {
          console.log(`  ${project.name} (${project.id})`);
        }
      }
      return;
    }

    // Validate file exists
    const filePath = resolve(options['file']);
    if (!existsSync(filePath)) {
      console.error(`Error: Documentation file not found: ${filePath}`);
      Deno.exit(1);
    }

    console.log(`\nParsing documentation: ${filePath}`);
    const document = parseMarkdownFile(filePath);
    console.log(`  Title: ${document.title}`);

    // Extract data from document
    const roadmapItems = extractRoadmapItems(document);
    const components = extractComponents(document);
    const sections = flattenSections(document);

    if (options['verbose']) {
      console.log(`\n  Sections: ${sections.length}`);
      console.log(`  Roadmap items: ${roadmapItems.length}`);
      console.log(`  Components: ${components.length}`);
    }

    console.log('\nRoadmap Items:');
    for (const item of roadmapItems) {
      const status = item.completed ? '[x]' : '[ ]';
      console.log(`  ${status} ${item.title}`);
    }

    console.log('\nComponents:');
    for (const component of components) {
      console.log(`  - ${component.name} (${component.path})`);
    }

    if (importConfig.dryRun) {
      console.log('\n=== DRY RUN MODE - No changes will be made ===\n');
    }

    // Perform import
    console.log('\nStarting import...');
    const result = await importer.importDocumentation(
      document,
      roadmapItems,
      components,
    );

    // Print results
    console.log('\n=== Import Results ===');
    if (result.projectId) {
      console.log(`Project: ${result.projectName} (${result.projectId})`);
    }
    console.log(`Issues created: ${result.issuesCreated}`);
    console.log(`Documents created: ${result.documentsCreated}`);

    if (result.errors.length > 0) {
      console.log(`\nErrors (${result.errors.length}):`);
      for (const error of result.errors) {
        console.log(`  - ${error}`);
      }
    }

    console.log('\nImport complete!');
  } catch (error) {
    console.error(`\nError: ${error}`);
    Deno.exit(1);
  }
}

main().catch((error) => {
  console.error('Fatal error:', error);
  Deno.exit(1);
});
