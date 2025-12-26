/**
 * Console module exports
 * Interactive mode components for the Rules Compiler
 */

// Utilities
export {
  bold,
  chalk,
  colorStatus,
  createKeyValueTable,
  createSpinner,
  createTable,
  dim,
  displayTable,
  formatBytes,
  formatElapsed,
  formatValue,
  generateBanner,
  showError,
  showHeader,
  showInfo,
  showNoItems,
  showPanel,
  showRule,
  showSuccess,
  showWarning,
  showWelcomeBanner,
  truncate,
  withSpinner,
} from './utils.ts';

// Application
export { ConsoleApplication, runInteractive } from './app.ts';
export type { ConsoleAppOptions } from './app.ts';
