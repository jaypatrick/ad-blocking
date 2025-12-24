/**
 * Models tests
 */

import { assertEquals } from '@std/assert';
import {
  DeviceType,
  BlockingMode,
  CategoryType,
  FilteringActionStatus,
  ErrorCodes,
  DayOfWeek,
} from '../../src/models/index.ts';

Deno.test('DeviceType enum - should have all device types', () => {
  assertEquals(DeviceType.WINDOWS, 'WINDOWS');
  assertEquals(DeviceType.ANDROID, 'ANDROID');
  assertEquals(DeviceType.MAC, 'MAC');
  assertEquals(DeviceType.IOS, 'IOS');
  assertEquals(DeviceType.LINUX, 'LINUX');
  assertEquals(DeviceType.ROUTER, 'ROUTER');
  assertEquals(DeviceType.SMART_TV, 'SMART_TV');
  assertEquals(DeviceType.GAME_CONSOLE, 'GAME_CONSOLE');
  assertEquals(DeviceType.UNKNOWN, 'UNKNOWN');
});

Deno.test('BlockingMode enum - should have all blocking modes', () => {
  assertEquals(BlockingMode.NONE, 'NONE');
  assertEquals(BlockingMode.NULL_IP, 'NULL_IP');
  assertEquals(BlockingMode.REFUSED, 'REFUSED');
  assertEquals(BlockingMode.NXDOMAIN, 'NXDOMAIN');
  assertEquals(BlockingMode.CUSTOM_IP, 'CUSTOM_IP');
});

Deno.test('CategoryType enum - should have all category types', () => {
  assertEquals(CategoryType.ADS, 'ADS');
  assertEquals(CategoryType.TRACKERS, 'TRACKERS');
  assertEquals(CategoryType.SOCIAL_MEDIA, 'SOCIAL_MEDIA');
  assertEquals(CategoryType.CDN, 'CDN');
  assertEquals(CategoryType.OTHERS, 'OTHERS');
});

Deno.test('FilteringActionStatus enum - should have all filtering statuses', () => {
  assertEquals(FilteringActionStatus.UNKNOWN, 'UNKNOWN');
  assertEquals(FilteringActionStatus.NONE, 'NONE');
  assertEquals(FilteringActionStatus.REQUEST_BLOCKED, 'REQUEST_BLOCKED');
  assertEquals(FilteringActionStatus.RESPONSE_BLOCKED, 'RESPONSE_BLOCKED');
  assertEquals(FilteringActionStatus.REQUEST_ALLOWED, 'REQUEST_ALLOWED');
  assertEquals(FilteringActionStatus.RESPONSE_ALLOWED, 'RESPONSE_ALLOWED');
  assertEquals(FilteringActionStatus.MODIFIED, 'MODIFIED');
});

Deno.test('ErrorCodes enum - should have all error codes', () => {
  assertEquals(ErrorCodes.BAD_REQUEST, 'BAD_REQUEST');
  assertEquals(ErrorCodes.FIELD_REQUIRED, 'FIELD_REQUIRED');
  assertEquals(ErrorCodes.FIELD_WRONG_VALUE, 'FIELD_WRONG_VALUE');
  assertEquals(ErrorCodes.FIELD_REACHED_LIMIT, 'FIELD_REACHED_LIMIT');
  assertEquals(ErrorCodes.UNKNOWN, 'UNKNOWN');
});

Deno.test('DayOfWeek enum - should have all days', () => {
  assertEquals(DayOfWeek.MONDAY, 'MONDAY');
  assertEquals(DayOfWeek.TUESDAY, 'TUESDAY');
  assertEquals(DayOfWeek.WEDNESDAY, 'WEDNESDAY');
  assertEquals(DayOfWeek.THURSDAY, 'THURSDAY');
  assertEquals(DayOfWeek.FRIDAY, 'FRIDAY');
  assertEquals(DayOfWeek.SATURDAY, 'SATURDAY');
  assertEquals(DayOfWeek.SUNDAY, 'SUNDAY');
});
