/**
 * CLI utility functions
 */

import chalk from 'chalk';
import Table from 'cli-table3';
import ora from 'ora';

/** Format a value for display */
export function formatValue(value: unknown): string {
  if (value === null || value === undefined) {
    return chalk.dim('N/A');
  }
  if (typeof value === 'boolean') {
    return value ? chalk.green('Yes') : chalk.red('No');
  }
  if (typeof value === 'number') {
    return chalk.cyan(value.toString());
  }
  if (Array.isArray(value)) {
    return value.length > 0 ? value.join(', ') : chalk.dim('None');
  }
  return String(value);
}

/** Create a standard table */
export function createTable(headers: string[]): Table.Table {
  return new Table({
    head: headers.map((h) => chalk.bold.white(h)),
    style: {
      head: [],
      border: [],
    },
  });
}

/** Display a table */
export function displayTable(table: Table.Table): void {
  console.log(table.toString());
}

/** Display a success message */
export function showSuccess(message: string): void {
  console.log(chalk.green('✓ ' + message));
}

/** Display an error message */
export function showError(message: string): void {
  console.log(chalk.red('✗ ' + message));
}

/** Display a warning message */
export function showWarning(message: string): void {
  console.log(chalk.yellow('⚠ ' + message));
}

/** Display an info message */
export function showInfo(message: string): void {
  console.log(chalk.blue('ℹ ' + message));
}

/** Display a header */
export function showHeader(title: string): void {
  console.log();
  console.log(chalk.bold.underline(title));
  console.log();
}

/** Display a panel with key-value pairs */
export function showPanel(title: string, data: Record<string, unknown>): void {
  console.log();
  console.log(chalk.bold.bgBlue.white(` ${title} `));
  console.log();
  for (const [key, value] of Object.entries(data)) {
    console.log(`  ${chalk.bold(key)}: ${formatValue(value)}`);
  }
  console.log();
}

/** Display no items message */
export function showNoItems(type: string): void {
  console.log(chalk.dim(`  No ${type} found.`));
}

/** Create a spinner */
export function createSpinner(text: string): import('ora').Ora {
  return ora({
    text,
    spinner: 'dots',
  });
}

/** Execute with spinner */
export async function withSpinner<T>(
  text: string,
  action: () => Promise<T>,
): Promise<T> {
  const spinner = createSpinner(text);
  spinner.start();
  try {
    const result = await action();
    spinner.succeed();
    return result;
  } catch (error) {
    spinner.fail();
    throw error;
  }
}

/** Format bytes to human readable */
export function formatBytes(bytes: number): string {
  const units = ['B', 'KB', 'MB', 'GB', 'TB'];
  let unitIndex = 0;
  let value = bytes;
  while (value >= 1024 && unitIndex < units.length - 1) {
    value /= 1024;
    unitIndex++;
  }
  return `${value.toFixed(1)} ${units[unitIndex]}`;
}

/** Format percentage */
export function formatPercentage(value: number): string {
  return `${value.toFixed(1)}%`;
}

/** Format date for display */
export function formatDate(timestamp: number): string {
  return new Date(timestamp).toLocaleString();
}

/** Format relative time */
export function formatRelativeTime(timestamp: number): string {
  const seconds = Math.floor((Date.now() - timestamp) / 1000);
  if (seconds < 60) return 'just now';
  if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
  if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
  return `${Math.floor(seconds / 86400)}d ago`;
}

/** Truncate string to max length */
export function truncate(str: string, maxLength: number): string {
  if (str.length <= maxLength) return str;
  return str.substring(0, maxLength - 3) + '...';
}
