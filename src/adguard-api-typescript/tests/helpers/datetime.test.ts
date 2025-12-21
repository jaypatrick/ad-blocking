/**
 * DateTime helper tests
 */

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
} from '../../src/helpers/datetime';

describe('DateTime helpers', () => {
  describe('toUnixMilliseconds', () => {
    it('should convert Date to Unix milliseconds', () => {
      const date = new Date('2024-01-15T12:00:00.000Z');
      const result = toUnixMilliseconds(date);
      expect(result).toBe(1705320000000);
    });
  });

  describe('fromUnixMilliseconds', () => {
    it('should convert Unix milliseconds to Date', () => {
      const millis = 1705320000000;
      const result = fromUnixMilliseconds(millis);
      expect(result.toISOString()).toBe('2024-01-15T12:00:00.000Z');
    });
  });

  describe('now', () => {
    it('should return current Unix milliseconds', () => {
      const before = Date.now();
      const result = now();
      const after = Date.now();
      expect(result).toBeGreaterThanOrEqual(before);
      expect(result).toBeLessThanOrEqual(after);
    });
  });

  describe('daysAgo', () => {
    it('should return Unix milliseconds for N days ago', () => {
      const days = 7;
      const expected = Date.now() - days * 24 * 60 * 60 * 1000;
      const result = daysAgo(days);
      expect(Math.abs(result - expected)).toBeLessThan(100);
    });
  });

  describe('hoursAgo', () => {
    it('should return Unix milliseconds for N hours ago', () => {
      const hours = 24;
      const expected = Date.now() - hours * 60 * 60 * 1000;
      const result = hoursAgo(hours);
      expect(Math.abs(result - expected)).toBeLessThan(100);
    });
  });

  describe('minutesAgo', () => {
    it('should return Unix milliseconds for N minutes ago', () => {
      const minutes = 30;
      const expected = Date.now() - minutes * 60 * 1000;
      const result = minutesAgo(minutes);
      expect(Math.abs(result - expected)).toBeLessThan(100);
    });
  });

  describe('startOfToday', () => {
    it('should return midnight of today', () => {
      const result = fromUnixMilliseconds(startOfToday());
      expect(result.getHours()).toBe(0);
      expect(result.getMinutes()).toBe(0);
      expect(result.getSeconds()).toBe(0);
      expect(result.getMilliseconds()).toBe(0);
    });
  });

  describe('endOfToday', () => {
    it('should return end of today', () => {
      const result = fromUnixMilliseconds(endOfToday());
      expect(result.getHours()).toBe(23);
      expect(result.getMinutes()).toBe(59);
      expect(result.getSeconds()).toBe(59);
      expect(result.getMilliseconds()).toBe(999);
    });
  });

  describe('startOfDay', () => {
    it('should return midnight of given date', () => {
      const date = new Date('2024-06-15T14:30:00.000Z');
      const result = fromUnixMilliseconds(startOfDay(date));
      expect(result.getHours()).toBe(0);
      expect(result.getMinutes()).toBe(0);
    });
  });

  describe('endOfDay', () => {
    it('should return end of given date', () => {
      const date = new Date('2024-06-15T14:30:00.000Z');
      const result = fromUnixMilliseconds(endOfDay(date));
      expect(result.getHours()).toBe(23);
      expect(result.getMinutes()).toBe(59);
    });
  });

  describe('formatAsIso8601', () => {
    it('should format as ISO 8601 string', () => {
      const millis = 1705320000000;
      const result = formatAsIso8601(millis);
      expect(result).toBe('2024-01-15T12:00:00.000Z');
    });
  });

  describe('formatRelative', () => {
    it('should return "just now" for recent timestamps', () => {
      const result = formatRelative(Date.now() - 30000);
      expect(result).toBe('just now');
    });

    it('should return minutes ago', () => {
      const result = formatRelative(Date.now() - 5 * 60 * 1000);
      expect(result).toBe('5 minutes ago');
    });

    it('should return hours ago', () => {
      const result = formatRelative(Date.now() - 3 * 60 * 60 * 1000);
      expect(result).toBe('3 hours ago');
    });

    it('should return days ago', () => {
      const result = formatRelative(Date.now() - 5 * 24 * 60 * 60 * 1000);
      expect(result).toBe('5 days ago');
    });
  });

  describe('DateTime object', () => {
    it('should export all functions', () => {
      expect(DateTime.toUnixMilliseconds).toBeDefined();
      expect(DateTime.fromUnixMilliseconds).toBeDefined();
      expect(DateTime.now).toBeDefined();
      expect(DateTime.daysAgo).toBeDefined();
      expect(DateTime.hoursAgo).toBeDefined();
      expect(DateTime.minutesAgo).toBeDefined();
      expect(DateTime.startOfToday).toBeDefined();
      expect(DateTime.endOfToday).toBeDefined();
      expect(DateTime.formatAsIso8601).toBeDefined();
      expect(DateTime.formatRelative).toBeDefined();
    });
  });
});
