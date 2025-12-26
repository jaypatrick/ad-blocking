/**
 * DateTime extension utilities for Unix millisecond timestamps
 * Matches .NET DateTimeExtensions functionality
 */

/** Convert Date to Unix milliseconds */
export function toUnixMilliseconds(date: Date): number {
  return date.getTime();
}

/** Convert Unix milliseconds to Date (UTC) */
export function fromUnixMilliseconds(millis: number): Date {
  return new Date(millis);
}

/** Get current Unix milliseconds */
export function now(): number {
  return Date.now();
}

/** Get Unix milliseconds from a relative time from now */
export function fromNow(milliseconds: number): number {
  return Date.now() + milliseconds;
}

/** Get Unix milliseconds for N days ago */
export function daysAgo(days: number): number {
  return Date.now() - days * 24 * 60 * 60 * 1000;
}

/** Get Unix milliseconds for N hours ago */
export function hoursAgo(hours: number): number {
  return Date.now() - hours * 60 * 60 * 1000;
}

/** Get Unix milliseconds for N minutes ago */
export function minutesAgo(minutes: number): number {
  return Date.now() - minutes * 60 * 1000;
}

/** Get Unix milliseconds for start of today (midnight) */
export function startOfToday(): number {
  const now = new Date();
  now.setHours(0, 0, 0, 0);
  return now.getTime();
}

/** Get Unix milliseconds for end of today (23:59:59.999) */
export function endOfToday(): number {
  const now = new Date();
  now.setHours(23, 59, 59, 999);
  return now.getTime();
}

/** Get Unix milliseconds for start of a specific day */
export function startOfDay(date: Date): number {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  return d.getTime();
}

/** Get Unix milliseconds for end of a specific day */
export function endOfDay(date: Date): number {
  const d = new Date(date);
  d.setHours(23, 59, 59, 999);
  return d.getTime();
}

/** Format Unix milliseconds as ISO 8601 string */
export function formatAsIso8601(millis: number): string {
  return new Date(millis).toISOString();
}

/** Format Unix milliseconds with custom format */
export function format(millis: number, options?: Intl.DateTimeFormatOptions): string {
  return new Date(millis).toLocaleString(undefined, options);
}

/** Format Unix milliseconds as relative time (e.g., "2 hours ago") */
export function formatRelative(millis: number): string {
  const seconds = Math.floor((Date.now() - millis) / 1000);

  if (seconds < 60) return 'just now';
  if (seconds < 3600) return `${Math.floor(seconds / 60)} minutes ago`;
  if (seconds < 86400) return `${Math.floor(seconds / 3600)} hours ago`;
  if (seconds < 2592000) return `${Math.floor(seconds / 86400)} days ago`;
  if (seconds < 31536000) return `${Math.floor(seconds / 2592000)} months ago`;
  return `${Math.floor(seconds / 31536000)} years ago`;
}

/** DateTime helper object with all utilities */
export const DateTime = {
  toUnixMilliseconds,
  fromUnixMilliseconds,
  now,
  fromNow,
  daysAgo,
  hoursAgo,
  minutesAgo,
  startOfToday,
  endOfToday,
  startOfDay,
  endOfDay,
  formatAsIso8601,
  format,
  formatRelative,
};

export default DateTime;
