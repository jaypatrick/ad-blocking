/**
 * Markdown documentation parser for Linear import
 */

import { readFileSync } from 'fs';
import { ComponentInfo, DocumentSection, ParsedDocument, RoadmapItem } from './types.ts';

/**
 * Parse a markdown file into structured sections
 */
export function parseMarkdownFile(filePath: string): ParsedDocument {
  const rawContent = readFileSync(filePath, 'utf-8');
  return parseMarkdown(rawContent);
}

/**
 * Parse markdown content into structured sections
 */
export function parseMarkdown(content: string): ParsedDocument {
  const lines = content.split('\n');
  const sections: DocumentSection[] = [];
  let title = '';

  let currentSection: DocumentSection | null = null;
  const sectionStack: DocumentSection[] = [];
  let contentBuffer: string[] = [];

  for (const line of lines) {
    const headingMatch = line.match(/^(#{1,6})\s+(.+)$/);

    if (headingMatch) {
      // Save accumulated content to current section
      if (currentSection) {
        currentSection.content = contentBuffer.join('\n').trim();
      }
      contentBuffer = [];

      const hashMarks = headingMatch[1];
      const sectionTitle = headingMatch[2];

      if (!hashMarks || !sectionTitle) {
        continue;
      }

      const level = hashMarks.length;

      // First h1 is the document title
      if (level === 1 && !title) {
        title = sectionTitle;
        continue;
      }

      const newSection: DocumentSection = {
        title: sectionTitle,
        level,
        content: '',
        subsections: [],
      };

      // Find parent section
      while (sectionStack.length > 0) {
        const lastSection = sectionStack[sectionStack.length - 1];
        if (lastSection && lastSection.level >= level) {
          sectionStack.pop();
        } else {
          break;
        }
      }

      if (sectionStack.length === 0) {
        sections.push(newSection);
      } else {
        const parentSection = sectionStack[sectionStack.length - 1];
        if (parentSection) {
          parentSection.subsections.push(newSection);
        }
      }

      sectionStack.push(newSection);
      currentSection = newSection;
    } else {
      contentBuffer.push(line);
    }
  }

  // Save final content
  if (currentSection) {
    currentSection.content = contentBuffer.join('\n').trim();
  }

  return {
    title,
    sections,
    rawContent: content,
  };
}

/**
 * Extract roadmap items from parsed document
 */
export function extractRoadmapItems(document: ParsedDocument): RoadmapItem[] {
  const roadmapItems: RoadmapItem[] = [];

  function findRoadmapSection(sections: DocumentSection[]): DocumentSection | null {
    for (const section of sections) {
      if (
        section.title.toLowerCase().includes('roadmap') ||
        section.title.toLowerCase().includes('future work')
      ) {
        return section;
      }
      const found = findRoadmapSection(section.subsections);
      if (found) return found;
    }
    return null;
  }

  const roadmapSection = findRoadmapSection(document.sections);
  if (!roadmapSection) return roadmapItems;

  // Parse checkbox items from roadmap section
  const content = roadmapSection.content +
    roadmapSection.subsections.map((s: DocumentSection) => s.content).join('\n');

  const checkboxRegex = /^-\s*\[([ x])\]\s*(.+)$/gm;
  let match;

  while ((match = checkboxRegex.exec(content)) !== null) {
    const checkbox = match[1];
    const itemTitle = match[2];
    if (checkbox !== undefined && itemTitle !== undefined) {
      roadmapItems.push({
        title: itemTitle.trim(),
        description: '',
        completed: checkbox === 'x',
      });
    }
  }

  return roadmapItems;
}

/**
 * Extract component information from parsed document
 */
export function extractComponents(document: ParsedDocument): ComponentInfo[] {
  const components: ComponentInfo[] = [];

  function findComponentsSection(sections: DocumentSection[]): DocumentSection | null {
    for (const section of sections) {
      if (section.title.toLowerCase() === 'components') {
        return section;
      }
      const found = findComponentsSection(section.subsections);
      if (found) return found;
    }
    return null;
  }

  const componentsSection = findComponentsSection(document.sections);
  if (!componentsSection) return components;

  for (const subsection of componentsSection.subsections) {
    // Parse component info from subsection
    const pathMatch = subsection.title.match(/\(([^)]+)\)/);
    const nameMatch = subsection.title.match(/^\d+\.\s*(.+?)(?:\s*\(|$)/);

    const component: ComponentInfo = {
      name: nameMatch?.[1]?.trim() ?? subsection.title,
      path: pathMatch?.[1] ?? '',
      purpose: '',
      techStack: [],
      keyFiles: [],
    };

    // Extract purpose from content
    const purposeMatch = subsection.content.match(/\*\*Purpose:\*\*\s*(.+)/);
    if (purposeMatch?.[1]) {
      component.purpose = purposeMatch[1].trim();
    }

    // Extract tech stack from content
    const techStackSection = subsection.content.match(
      /\*\*Technology Stack:\*\*\n([\s\S]*?)(?=\n\*\*|\n---|\n###|$)/,
    );
    if (techStackSection?.[1]) {
      const techItems = techStackSection[1].match(/^-\s+(.+)/gm);
      if (techItems) {
        component.techStack = techItems.map((item: string) => item.replace(/^-\s+/, '').trim());
      }
    }

    // Extract key files from tables
    const tableMatch = subsection.content.match(
      /\| File \| Description \|\n\|[-\s|]+\|\n([\s\S]*?)(?=\n\n|\n---|\n###|$)/,
    );
    if (tableMatch?.[1]) {
      const rows = tableMatch[1].split('\n').filter((row: string) => row.trim());
      for (const row of rows) {
        const cells = row.split('|').filter((cell: string) => cell.trim());
        if (cells.length >= 2 && cells[0] && cells[1]) {
          component.keyFiles.push({
            file: cells[0].replace(/`/g, '').trim(),
            description: cells[1].trim(),
          });
        }
      }
    }

    components.push(component);
  }

  return components;
}

/**
 * Get section by title path (e.g., "Components/Filter Rules")
 */
export function getSectionByPath(
  document: ParsedDocument,
  path: string,
): DocumentSection | null {
  const parts = path.split('/');
  let sections = document.sections;

  for (let i = 0; i < parts.length; i++) {
    const part = parts[i];
    if (!part) continue;

    const found = sections.find((s: DocumentSection) =>
      s.title.toLowerCase().includes(part.toLowerCase())
    );
    if (!found) return null;
    if (i === parts.length - 1) return found;
    sections = found.subsections;
  }

  return null;
}

/**
 * Flatten all sections into a single array
 */
export function flattenSections(document: ParsedDocument): DocumentSection[] {
  const result: DocumentSection[] = [];

  function flatten(sections: DocumentSection[]): void {
    for (const section of sections) {
      result.push(section);
      flatten(section.subsections);
    }
  }

  flatten(document.sections);
  return result;
}
