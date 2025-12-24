/**
 * DateTime helper tests
 */

import {
  assertEquals,
  assertGreaterOrEqual,
  assertLessOrEqual,
  assertLess,
  assertExists,
} from '@std/assert';
import {
  DateTime,
  toUnixMilliseconds,
  fromUnixMilliseconds,
  now,
  daysAgo,
  hoursAgo,
  minutesAgo,
  startOfToday,
  endOfToday,
  startOfDay,
  endOfDay,
  formatAsIso8601,
  formatRelative,
} from '../../src/helpers/datetime.ts';

// toUnixMilliseconds tests
Deno.test('toUnixMilliseconds - should convert Date to Unix milliseconds', () => {
  const date = new Date('2024-01-15T12:00:00.000Z');
  const result = toUnixMilliseconds(date);
  assertEquals(result, 1705320000000);
});

// fromUnixMilliseconds tests
Deno.test('fromUnixMilliseconds - should convert Unix milliseconds to Date', () => {
  const millis = 1705320000000;
  const result = fromUnixMilliseconds(millis);
  assertEquals(result.toISOString(), '2024-01-15T12:00:00.000Z');
});

// now tests
Deno.test('now - should return current Unix milliseconds', () => {
  const before = Date.now();
  const result = now();
  const after = Date.now();
  assertGreaterOrEqual(result, before);
  assertLessOrEqual(result, after);
});

// daysAgo tests
Deno.test('daysAgo - should return Unix milliseconds for N days ago', () => {
  const days = 7;
  const expected = Date.now() - days * 24 * 60 * 60 * 1000;
  const result = daysAgo(days);
  assertLess(Math.abs(result - expected), 100);
});

// hoursAgo tests
Deno.test('hoursAgo - should return Unix milliseconds for N hours ago', () => {
  const hours = 24;
  const expected = Date.now() - hours * 60 * 60 * 1000;
  const result = hoursAgo(hours);
  assertLess(Math.abs(result - expected), 100);
});

// minutesAgo tests
Deno.test('minutesAgo - should return Unix milliseconds for N minutes ago', () => {
  const minutes = 30;
  const expected = Date.now() - minutes * 60 * 1000;
  const result = minutesAgo(minutes);
  assertLess(Math.abs(result - expected), 100);
});

// startOfToday tests
Deno.test('startOfToday - should return midnight of today', () => {
  const result = fromUnixMilliseconds(startOfToday());
  assertEquals(result.getHours(), 0);
  assertEquals(result.getMinutes(), 0);
  assertEquals(result.getSeconds(), 0);
  assertEquals(result.getMilliseconds(), 0);
});

// endOfToday tests
Deno.test('endOfToday - should return end of today', () => {
  const result = fromUnixMilliseconds(endOfToday());
  assertEquals(result.getHours(), 23);
  assertEquals(result.getMinutes(), 59);
  assertEquals(result.getSeconds(), 59);
  assertEquals(result.getMilliseconds(), 999);
});

// startOfDay tests
Deno.test('startOfDay - should return midnight of given date', () => {
  const date = new Date('2024-06-15T14:30:00.000Z');
  const result = fromUnixMilliseconds(startOfDay(date));
  assertEquals(result.getHours(), 0);
  assertEquals(result.getMinutes(), 0);
});

// endOfDay tests
Deno.test('endOfDay - should return end of given date', () => {
  const date = new Date('2024-06-15T14:30:00.000Z');
  const result = fromUnixMilliseconds(endOfDay(date));
  assertEquals(result.getHours(), 23);
  assertEquals(result.getMinutes(), 59);
});

// formatAsIso8601 tests
Deno.test('formatAsIso8601 - should format as ISO 8601 string', () => {
  const millis = 1705320000000;
  const result = formatAsIso8601(millis);
  assertEquals(result, '2024-01-15T12:00:00.000Z');
});

// formatRelative tests
Deno.test('formatRelative - should return just now for recent timestamps', () => {
  const result = formatRelative(Date.now() - 30000);
  assertEquals(result, 'just now');
});

Deno.test('formatRelative - should return minutes ago', () => {
  const result = formatRelative(Date.now() - 5 * 60 * 1000);
  assertEquals(result, '5 minutes ago');
});

Deno.test('formatRelative - should return hours ago', () => {
  const result = formatRelative(Date.now() - 3 * 60 * 60 * 1000);
  assertEquals(result, '3 hours ago');
});

Deno.test('formatRelative - should return days ago', () => {
  const result = formatRelative(Date.now() - 5 * 24 * 60 * 60 * 1000);
  assertEquals(result, '5 days ago');
});

// DateTime object tests
Deno.test('DateTime object - should export all functions', () => {
  assertExists(DateTime.toUnixMilliseconds);
  assertExists(DateTime.fromUnixMilliseconds);
  assertExists(DateTime.now);
  assertExists(DateTime.daysAgo);
  assertExists(DateTime.hoursAgo);
  assertExists(DateTime.minutesAgo);
  assertExists(DateTime.startOfToday);
  assertExists(DateTime.endOfToday);
  assertExists(DateTime.formatAsIso8601);
  assertExists(DateTime.formatRelative);
});
