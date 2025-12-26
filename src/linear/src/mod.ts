/**
 * Linear Documentation Import Tool - Deno Entry Point
 *
 * This module provides Deno-compatible exports and CLI entry point.
 * Uses Deno's Node.js compatibility layer for file system operations.
 *
 * @module
 */

// Re-export modules for library usage
export * from './types.ts';
export {
  extractComponents,
  extractRoadmapItems,
  flattenSections,
  getSectionByPath,
  parseMarkdown,
  parseMarkdownFile,
} from './parser.ts';
export { LinearImporter } from './linear-client.ts';

// Deno CLI entry point
if (import.meta.main) {
  const { Command } = await import('commander');
  const { resolve, dirname } = await import('node:path');
  const { fileURLToPath } = await import('node:url');
  const { existsSync } = await import('node:fs');

  const { LinearImporter } = await import('./linear-client.ts');
  const {
    parseMarkdownFile,
    extractRoadmapItems,
    extractComponents,
    flattenSections,
  } = await import('./parser.ts');

  // Type definition for ImportConfig
  interface ImportConfig {
    teamId: string;
    projectName: string;
    createProject: boolean;
    createIssues: boolean;
    createDocuments: boolean;
    dryRun: boolean;
  }

  // Get module directory for default paths
  const __dirname = dirname(fileURLToPath(import.meta.url));
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
      .option('-v, --verbose', 'Verbose output');

    await program.parseAsync(Deno.args, { from: 'user' });

    const options = program.opts();

    // Check for API key
    const apiKey = Deno.env.get('LINEAR_API_KEY');
    if (!apiKey) {
      console.error('Error: LINEAR_API_KEY environment variable is required');
      console.error('\nTo get your API key:');
      console.error('1. Go to Linear Settings > API');
      console.error('2. Create a new personal API key');
      console.error('3. Set it in your .env file or environment');
      Deno.exit(1);
    }

    const importConfig: ImportConfig = {
      teamId: options['team'] || Deno.env.get('LINEAR_TEAM_ID') || '',
      projectName: options['project'] || Deno.env.get('LINEAR_PROJECT_NAME') || '',
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
}
