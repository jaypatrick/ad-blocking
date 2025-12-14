/**
 * Linear API client wrapper for documentation import
 */

import { LinearClient, Project, Issue, Team, Document } from "@linear/sdk";
import {
  ImportConfig,
  LinearImportResult,
  ParsedDocument,
  RoadmapItem,
  ComponentInfo,
} from "./types.js";

export class LinearImporter {
  private client: LinearClient;
  private config: ImportConfig;
  private teamId: string | null = null;

  constructor(apiKey: string, config: ImportConfig) {
    this.client = new LinearClient({ apiKey });
    this.config = config;
  }

  /**
   * Initialize and validate the Linear connection
   */
  async initialize(): Promise<void> {
    try {
      const viewer = await this.client.viewer;
      console.log(`Connected to Linear as: ${viewer.name || viewer.email}`);

      // Get or validate team
      if (this.config.teamId) {
        this.teamId = this.config.teamId;
      } else {
        const teams = await this.client.teams();
        if (teams.nodes.length === 0) {
          throw new Error("No teams found in Linear workspace");
        }
        this.teamId = teams.nodes[0].id;
        console.log(`Using team: ${teams.nodes[0].name}`);
      }
    } catch (error) {
      throw new Error(`Failed to initialize Linear client: ${error}`);
    }
  }

  /**
   * Import documentation into Linear
   */
  async importDocumentation(
    document: ParsedDocument,
    roadmapItems: RoadmapItem[],
    components: ComponentInfo[]
  ): Promise<LinearImportResult> {
    const result: LinearImportResult = {
      issuesCreated: 0,
      documentsCreated: 0,
      errors: [],
    };

    if (!this.teamId) {
      throw new Error("Linear client not initialized. Call initialize() first.");
    }

    try {
      // Create or get project
      let projectId: string | undefined;
      if (this.config.createProject) {
        projectId = await this.createOrGetProject(document.title);
        result.projectId = projectId;
        result.projectName = this.config.projectName || document.title;
      }

      // Create roadmap issues
      if (this.config.createIssues && roadmapItems.length > 0) {
        console.log(`\nCreating ${roadmapItems.length} roadmap issues...`);
        for (const item of roadmapItems) {
          try {
            if (this.config.dryRun) {
              console.log(`  [DRY RUN] Would create issue: ${item.title}`);
              result.issuesCreated++;
            } else {
              await this.createIssue(item, projectId);
              result.issuesCreated++;
              console.log(`  Created issue: ${item.title}`);
            }
          } catch (error) {
            result.errors.push(`Failed to create issue "${item.title}": ${error}`);
          }
        }
      }

      // Create component documentation issues
      if (this.config.createIssues && components.length > 0) {
        console.log(`\nCreating ${components.length} component documentation issues...`);
        for (const component of components) {
          try {
            if (this.config.dryRun) {
              console.log(`  [DRY RUN] Would create component doc: ${component.name}`);
              result.documentsCreated++;
            } else {
              await this.createComponentIssue(component, projectId);
              result.documentsCreated++;
              console.log(`  Created component documentation: ${component.name}`);
            }
          } catch (error) {
            result.errors.push(
              `Failed to create component doc "${component.name}": ${error}`
            );
          }
        }
      }

      // Create main documentation issue
      if (this.config.createDocuments) {
        console.log(`\nCreating main documentation issue...`);
        try {
          if (this.config.dryRun) {
            console.log(`  [DRY RUN] Would create main documentation issue`);
            result.documentsCreated++;
          } else {
            await this.createMainDocumentationIssue(document, projectId);
            result.documentsCreated++;
            console.log(`  Created main documentation issue`);
          }
        } catch (error) {
          result.errors.push(`Failed to create main documentation: ${error}`);
        }
      }
    } catch (error) {
      result.errors.push(`Import failed: ${error}`);
    }

    return result;
  }

  /**
   * Create or get a project by name
   */
  private async createOrGetProject(defaultName: string): Promise<string> {
    const projectName = this.config.projectName || defaultName;

    // Check if project already exists
    const projects = await this.client.projects({
      filter: {
        name: { eq: projectName },
      },
    });

    if (projects.nodes.length > 0) {
      console.log(`Found existing project: ${projectName}`);
      return projects.nodes[0].id;
    }

    // Create new project
    if (this.config.dryRun) {
      console.log(`[DRY RUN] Would create project: ${projectName}`);
      return "dry-run-project-id";
    }

    const teams = await this.client.teams();
    const team = teams.nodes.find((t) => t.id === this.teamId) || teams.nodes[0];

    const projectPayload = await this.client.createProject({
      name: projectName,
      description: "Ad-blocking system documentation and tracking",
      teamIds: [team.id],
    });

    const project = await projectPayload.project;
    if (!project) {
      throw new Error("Failed to create project");
    }

    console.log(`Created project: ${projectName}`);
    return project.id;
  }

  /**
   * Create an issue from a roadmap item
   */
  private async createIssue(
    item: RoadmapItem,
    projectId?: string
  ): Promise<void> {
    const issueInput: {
      title: string;
      description: string;
      teamId: string;
      projectId?: string;
    } = {
      title: item.title,
      description: item.description || `Roadmap item: ${item.title}`,
      teamId: this.teamId!,
    };

    if (projectId) {
      issueInput.projectId = projectId;
    }

    await this.client.createIssue(issueInput);
  }

  /**
   * Create an issue for component documentation
   */
  private async createComponentIssue(
    component: ComponentInfo,
    projectId?: string
  ): Promise<void> {
    const description = this.formatComponentDescription(component);

    const issueInput: {
      title: string;
      description: string;
      teamId: string;
      projectId?: string;
    } = {
      title: `[Component] ${component.name}`,
      description,
      teamId: this.teamId!,
    };

    if (projectId) {
      issueInput.projectId = projectId;
    }

    await this.client.createIssue(issueInput);
  }

  /**
   * Create the main documentation issue
   */
  private async createMainDocumentationIssue(
    document: ParsedDocument,
    projectId?: string
  ): Promise<void> {
    // Truncate content if too long for Linear
    const maxLength = 10000;
    let content = document.rawContent;
    if (content.length > maxLength) {
      content = content.substring(0, maxLength) + "\n\n... (truncated)";
    }

    const issueInput: {
      title: string;
      description: string;
      teamId: string;
      projectId?: string;
    } = {
      title: `[Documentation] ${document.title}`,
      description: content,
      teamId: this.teamId!,
    };

    if (projectId) {
      issueInput.projectId = projectId;
    }

    await this.client.createIssue(issueInput);
  }

  /**
   * Format component info as markdown description
   */
  private formatComponentDescription(component: ComponentInfo): string {
    let description = `## ${component.name}\n\n`;
    description += `**Path:** \`${component.path}\`\n\n`;

    if (component.purpose) {
      description += `**Purpose:** ${component.purpose}\n\n`;
    }

    if (component.techStack.length > 0) {
      description += `### Technology Stack\n`;
      for (const tech of component.techStack) {
        description += `- ${tech}\n`;
      }
      description += "\n";
    }

    if (component.keyFiles.length > 0) {
      description += `### Key Files\n`;
      description += "| File | Description |\n";
      description += "|------|-------------|\n";
      for (const file of component.keyFiles) {
        description += `| \`${file.file}\` | ${file.description} |\n`;
      }
    }

    return description;
  }

  /**
   * List available teams
   */
  async listTeams(): Promise<Array<{ id: string; name: string }>> {
    const teams = await this.client.teams();
    return teams.nodes.map((team) => ({
      id: team.id,
      name: team.name,
    }));
  }

  /**
   * List existing projects
   */
  async listProjects(): Promise<Array<{ id: string; name: string }>> {
    const projects = await this.client.projects();
    return projects.nodes.map((project) => ({
      id: project.id,
      name: project.name,
    }));
  }
}
