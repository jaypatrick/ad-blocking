/**
 * DateTime helper tests
 */

import { assertEquals, assert } from '@std/assert';
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

Deno.test('DateTime helpers', async (t) => {
  await t.step('toUnixMilliseconds', async (t) => {
    await t.step('should convert Date to Unix milliseconds', () => {
      const date = new Date('2024-01-15T12:00:00.000Z');
      const result = toUnixMilliseconds(date);
      assertEquals(result, 1705320000000);
    });
  });

  await t.step('fromUnixMilliseconds', async (t) => {
    await t.step('should convert Unix milliseconds to Date', () => {
      const millis = 1705320000000;
      const result = fromUnixMilliseconds(millis);
      assertEquals(result.toISOString(), '2024-01-15T12:00:00.000Z');
    });
  });

  await t.step('now', async (t) => {
    await t.step('should return current Unix milliseconds', () => {
      const before = Date.now();
      const result = now();
      const after = Date.now();
      assert(result >= before);
      assert(result <= after);
    });
  });

  await t.step('daysAgo', async (t) => {
    await t.step('should return Unix milliseconds for N days ago', () => {
      const days = 7;
      const expected = Date.now() - days * 24 * 60 * 60 * 1000;
      const result = daysAgo(days);
      assert(Math.abs(result - expected) < 100);
    });
  });

  await t.step('hoursAgo', async (t) => {
    await t.step('should return Unix milliseconds for N hours ago', () => {
      const hours = 24;
      const expected = Date.now() - hours * 60 * 60 * 1000;
      const result = hoursAgo(hours);
      assert(Math.abs(result - expected) < 100);
    });
  });

  await t.step('minutesAgo', async (t) => {
    await t.step('should return Unix milliseconds for N minutes ago', () => {
      const minutes = 30;
      const expected = Date.now() - minutes * 60 * 1000;
      const result = minutesAgo(minutes);
      assert(Math.abs(result - expected) < 100);
    });
  });

  await t.step('startOfToday', async (t) => {
    await t.step('should return midnight of today', () => {
      const result = fromUnixMilliseconds(startOfToday());
      assertEquals(result.getHours(), 0);
      assertEquals(result.getMinutes(), 0);
      assertEquals(result.getSeconds(), 0);
      assertEquals(result.getMilliseconds(), 0);
    });
  });

  await t.step('endOfToday', async (t) => {
    await t.step('should return end of today', () => {
      const result = fromUnixMilliseconds(endOfToday());
      assertEquals(result.getHours(), 23);
      assertEquals(result.getMinutes(), 59);
      assertEquals(result.getSeconds(), 59);
      assertEquals(result.getMilliseconds(), 999);
    });
  });

  await t.step('startOfDay', async (t) => {
    await t.step('should return midnight of given date', () => {
      const date = new Date('2024-06-15T14:30:00.000Z');
      const result = fromUnixMilliseconds(startOfDay(date));
      assertEquals(result.getHours(), 0);
      assertEquals(result.getMinutes(), 0);
    });
  });

  await t.step('endOfDay', async (t) => {
    await t.step('should return end of given date', () => {
      const date = new Date('2024-06-15T14:30:00.000Z');
      const result = fromUnixMilliseconds(endOfDay(date));
      assertEquals(result.getHours(), 23);
      assertEquals(result.getMinutes(), 59);
    });
  });

  await t.step('formatAsIso8601', async (t) => {
    await t.step('should format as ISO 8601 string', () => {
      const millis = 1705320000000;
      const result = formatAsIso8601(millis);
      assertEquals(result, '2024-01-15T12:00:00.000Z');
    });
  });

  await t.step('formatRelative', async (t) => {
    await t.step('should return "just now" for recent timestamps', () => {
      const result = formatRelative(Date.now() - 30000);
      assertEquals(result, 'just now');
    });

    await t.step('should return minutes ago', () => {
      const result = formatRelative(Date.now() - 5 * 60 * 1000);
      assertEquals(result, '5 minutes ago');
    });

    await t.step('should return hours ago', () => {
      const result = formatRelative(Date.now() - 3 * 60 * 60 * 1000);
      assertEquals(result, '3 hours ago');
    });

    await t.step('should return days ago', () => {
      const result = formatRelative(Date.now() - 5 * 24 * 60 * 60 * 1000);
      assertEquals(result, '5 days ago');
    });
  });

  await t.step('DateTime object', async (t) => {
    await t.step('should export all functions', () => {
      assert(DateTime.toUnixMilliseconds !== undefined);
      assert(DateTime.fromUnixMilliseconds !== undefined);
      assert(DateTime.now !== undefined);
      assert(DateTime.daysAgo !== undefined);
      assert(DateTime.hoursAgo !== undefined);
      assert(DateTime.minutesAgo !== undefined);
      assert(DateTime.startOfToday !== undefined);
      assert(DateTime.endOfToday !== undefined);
      assert(DateTime.formatAsIso8601 !== undefined);
      assert(DateTime.formatRelative !== undefined);
    });
  });
});
