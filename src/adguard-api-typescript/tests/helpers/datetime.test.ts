/**
 * DateTime helper tests
 * Deno-native testing implementation
 */

import {
  assertEquals,
  assertAlmostEquals,
  assertLessOrEqual,
  assertGreaterOrEqual,
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

Deno.test('toUnixMilliseconds - converts Date to Unix milliseconds', () => {
  const date = new Date('2024-01-15T12:00:00.000Z');
  const result = toUnixMilliseconds(date);
  assertEquals(result, 1705320000000);
});

Deno.test('fromUnixMilliseconds - converts Unix milliseconds to Date', () => {
  const millis = 1705320000000;
  const result = fromUnixMilliseconds(millis);
  assertEquals(result.toISOString(), '2024-01-15T12:00:00.000Z');
});

Deno.test('now - returns current Unix milliseconds', () => {
  const before = Date.now();
  const result = now();
  const after = Date.now();
  assertGreaterOrEqual(result, before);
  assertLessOrEqual(result, after);
});

Deno.test('daysAgo - returns Unix milliseconds for N days ago', () => {
  const days = 7;
  const expected = Date.now() - days * 24 * 60 * 60 * 1000;
  const result = daysAgo(days);
  assertAlmostEquals(result, expected, 100);
});

Deno.test('hoursAgo - returns Unix milliseconds for N hours ago', () => {
  const hours = 24;
  const expected = Date.now() - hours * 60 * 60 * 1000;
  const result = hoursAgo(hours);
  assertAlmostEquals(result, expected, 100);
});

Deno.test('minutesAgo - returns Unix milliseconds for N minutes ago', () => {
  const minutes = 30;
  const expected = Date.now() - minutes * 60 * 1000;
  const result = minutesAgo(minutes);
  assertAlmostEquals(result, expected, 100);
});

Deno.test('startOfToday - returns midnight of today', () => {
  const result = fromUnixMilliseconds(startOfToday());
  assertEquals(result.getHours(), 0);
  assertEquals(result.getMinutes(), 0);
  assertEquals(result.getSeconds(), 0);
  assertEquals(result.getMilliseconds(), 0);
});

Deno.test('endOfToday - returns end of today', () => {
  const result = fromUnixMilliseconds(endOfToday());
  assertEquals(result.getHours(), 23);
  assertEquals(result.getMinutes(), 59);
  assertEquals(result.getSeconds(), 59);
  assertEquals(result.getMilliseconds(), 999);
});

Deno.test('startOfDay - returns midnight of given date', () => {
  const date = new Date('2024-06-15T14:30:00.000Z');
  const result = fromUnixMilliseconds(startOfDay(date));
  assertEquals(result.getHours(), 0);
  assertEquals(result.getMinutes(), 0);
});

Deno.test('endOfDay - returns end of given date', () => {
  const date = new Date('2024-06-15T14:30:00.000Z');
  const result = fromUnixMilliseconds(endOfDay(date));
  assertEquals(result.getHours(), 23);
  assertEquals(result.getMinutes(), 59);
});

Deno.test('formatAsIso8601 - formats as ISO 8601 string', () => {
  const millis = 1705320000000;
  const result = formatAsIso8601(millis);
  assertEquals(result, '2024-01-15T12:00:00.000Z');
});

Deno.test('formatRelative - returns "just now" for recent timestamps', () => {
  const result = formatRelative(Date.now() - 30000);
  assertEquals(result, 'just now');
});

Deno.test('formatRelative - returns minutes ago', () => {
  const result = formatRelative(Date.now() - 5 * 60 * 1000);
  assertEquals(result, '5 minutes ago');
});

Deno.test('formatRelative - returns hours ago', () => {
  const result = formatRelative(Date.now() - 3 * 60 * 60 * 1000);
  assertEquals(result, '3 hours ago');
});

Deno.test('formatRelative - returns days ago', () => {
  const result = formatRelative(Date.now() - 5 * 24 * 60 * 60 * 1000);
  assertEquals(result, '5 days ago');
});

Deno.test('DateTime object - exports all functions', () => {
  assertEquals(typeof DateTime.toUnixMilliseconds, 'function');
  assertEquals(typeof DateTime.fromUnixMilliseconds, 'function');
  assertEquals(typeof DateTime.now, 'function');
  assertEquals(typeof DateTime.daysAgo, 'function');
  assertEquals(typeof DateTime.hoursAgo, 'function');
  assertEquals(typeof DateTime.minutesAgo, 'function');
  assertEquals(typeof DateTime.startOfToday, 'function');
  assertEquals(typeof DateTime.endOfToday, 'function');
  assertEquals(typeof DateTime.formatAsIso8601, 'function');
  assertEquals(typeof DateTime.formatRelative, 'function');
});
