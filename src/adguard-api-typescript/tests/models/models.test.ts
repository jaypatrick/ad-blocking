/**
 * Models tests
 */

import {
  DeviceType,
  BlockingMode,
  CategoryType,
  FilteringActionStatus,
  ErrorCodes,
  DayOfWeek,
} from '../../src/models';

describe('Models', () => {
  describe('DeviceType enum', () => {
    it('should have all device types', () => {
      expect(DeviceType.WINDOWS).toBe('WINDOWS');
      expect(DeviceType.ANDROID).toBe('ANDROID');
      expect(DeviceType.MAC).toBe('MAC');
      expect(DeviceType.IOS).toBe('IOS');
      expect(DeviceType.LINUX).toBe('LINUX');
      expect(DeviceType.ROUTER).toBe('ROUTER');
      expect(DeviceType.SMART_TV).toBe('SMART_TV');
      expect(DeviceType.GAME_CONSOLE).toBe('GAME_CONSOLE');
      expect(DeviceType.UNKNOWN).toBe('UNKNOWN');
    });
  });

  describe('BlockingMode enum', () => {
    it('should have all blocking modes', () => {
      expect(BlockingMode.NONE).toBe('NONE');
      expect(BlockingMode.NULL_IP).toBe('NULL_IP');
      expect(BlockingMode.REFUSED).toBe('REFUSED');
      expect(BlockingMode.NXDOMAIN).toBe('NXDOMAIN');
      expect(BlockingMode.CUSTOM_IP).toBe('CUSTOM_IP');
    });
  });

  describe('CategoryType enum', () => {
    it('should have all category types', () => {
      expect(CategoryType.ADS).toBe('ADS');
      expect(CategoryType.TRACKERS).toBe('TRACKERS');
      expect(CategoryType.SOCIAL_MEDIA).toBe('SOCIAL_MEDIA');
      expect(CategoryType.CDN).toBe('CDN');
      expect(CategoryType.OTHERS).toBe('OTHERS');
    });
  });

  describe('FilteringActionStatus enum', () => {
    it('should have all filtering statuses', () => {
      expect(FilteringActionStatus.UNKNOWN).toBe('UNKNOWN');
      expect(FilteringActionStatus.NONE).toBe('NONE');
      expect(FilteringActionStatus.REQUEST_BLOCKED).toBe('REQUEST_BLOCKED');
      expect(FilteringActionStatus.RESPONSE_BLOCKED).toBe('RESPONSE_BLOCKED');
      expect(FilteringActionStatus.REQUEST_ALLOWED).toBe('REQUEST_ALLOWED');
      expect(FilteringActionStatus.RESPONSE_ALLOWED).toBe('RESPONSE_ALLOWED');
      expect(FilteringActionStatus.MODIFIED).toBe('MODIFIED');
    });
  });

  describe('ErrorCodes enum', () => {
    it('should have all error codes', () => {
      expect(ErrorCodes.BAD_REQUEST).toBe('BAD_REQUEST');
      expect(ErrorCodes.FIELD_REQUIRED).toBe('FIELD_REQUIRED');
      expect(ErrorCodes.FIELD_WRONG_VALUE).toBe('FIELD_WRONG_VALUE');
      expect(ErrorCodes.FIELD_REACHED_LIMIT).toBe('FIELD_REACHED_LIMIT');
      expect(ErrorCodes.UNKNOWN).toBe('UNKNOWN');
    });
  });

  describe('DayOfWeek enum', () => {
    it('should have all days', () => {
      expect(DayOfWeek.MONDAY).toBe('MONDAY');
      expect(DayOfWeek.TUESDAY).toBe('TUESDAY');
      expect(DayOfWeek.WEDNESDAY).toBe('WEDNESDAY');
      expect(DayOfWeek.THURSDAY).toBe('THURSDAY');
      expect(DayOfWeek.FRIDAY).toBe('FRIDAY');
      expect(DayOfWeek.SATURDAY).toBe('SATURDAY');
      expect(DayOfWeek.SUNDAY).toBe('SUNDAY');
    });
  });
});
