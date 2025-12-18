/**
 * Types for Linear documentation import
 */

export interface DocumentSection {
  title: string;
  level: number;
  content: string;
  subsections: DocumentSection[];
}

export interface ParsedDocument {
  title: string;
  sections: DocumentSection[];
  rawContent: string;
}

export interface ImportConfig {
  teamId: string;
  projectName: string;
  createProject: boolean;
  createIssues: boolean;
  createDocuments: boolean;
  dryRun: boolean;
}

export interface RoadmapItem {
  title: string;
  description: string;
  completed: boolean;
}

export interface ComponentInfo {
  name: string;
  path: string;
  purpose: string;
  techStack: string[];
  keyFiles: Array<{ file: string; description: string }>;
}

export interface LinearImportResult {
  projectId?: string;
  projectName?: string;
  issuesCreated: number;
  documentsCreated: number;
  errors: string[];
}
