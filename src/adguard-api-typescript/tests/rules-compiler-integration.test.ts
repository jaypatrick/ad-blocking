/**
 * Rules compiler integration tests
 */

import { jest } from '@jest/globals';
import { RulesCompilerIntegration } from '../src/rules-compiler-integration';
import { UserRulesRepository } from '../src/repositories/user-rules';
import { DnsServerRepository } from '../src/repositories/dns-server';

// Mock the repositories
const mockUserRulesRepo = {
  getRules: jest.fn(),
  setRules: jest.fn(),
  enableRules: jest.fn(),
  disableRules: jest.fn(),
} as unknown as UserRulesRepository;

const mockDnsServerRepo = {
  getDefault: jest.fn(),
  getAll: jest.fn(),
} as unknown as DnsServerRepository;

describe('RulesCompilerIntegration', () => {
  let integration: RulesCompilerIntegration;

  beforeEach(() => {
    jest.clearAllMocks();
    integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);
  });

  describe('parseRules', () => {
    it('should parse rules from content', () => {
      const content = `||example.com^
||ads.example.org^
||tracker.net^`;

      const rules = integration.parseRules(content);
      expect(rules).toHaveLength(3);
      expect(rules[0]).toBe('||example.com^');
    });

    it('should filter comments when requested', () => {
      const content = `! This is a comment
||example.com^
# Another comment
||ads.example.org^`;

      const rules = integration.parseRules(content, true);
      expect(rules).toHaveLength(2);
      expect(rules).not.toContain('! This is a comment');
    });

    it('should keep comments when not filtering', () => {
      const content = `! Comment
||example.com^`;

      const rules = integration.parseRules(content, false);
      expect(rules).toHaveLength(2);
    });

    it('should filter empty lines', () => {
      const content = `||example.com^

||ads.example.org^

`;

      const rules = integration.parseRules(content, true);
      expect(rules).toHaveLength(2);
    });
  });

  describe('validateRules', () => {
    it('should validate correct rules', () => {
      const rules = ['||example.com^', '||ads.example.org^'];
      const result = integration.validateRules(rules);
      expect(result.valid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should detect rules exceeding max length', () => {
      const longDomain = 'a'.repeat(1100);
      const rules = [`||${longDomain}^`];
      const result = integration.validateRules(rules);
      expect(result.valid).toBe(false);
      expect(result.errors).toHaveLength(1);
      expect(result.errors[0].error).toContain('exceeds maximum length');
    });

    it('should detect invalid space in domain rules', () => {
      const rules = ['||example.com with space^'];
      const result = integration.validateRules(rules);
      expect(result.valid).toBe(false);
      expect(result.errors[0].error).toContain('invalid space');
    });

    it('should skip comments in validation', () => {
      const rules = ['! This is a comment', '||example.com^'];
      const result = integration.validateRules(rules);
      expect(result.valid).toBe(true);
    });
  });

  describe('syncRules', () => {
    it('should sync inline rules', async () => {
      (mockUserRulesRepo.setRules as jest.Mock).mockResolvedValue(undefined);
      (mockUserRulesRepo.enableRules as jest.Mock).mockResolvedValue(undefined);

      const result = await integration.syncRules('server-1', {
        rules: ['||example.com^', '||ads.org^'],
      });

      expect(result.success).toBe(true);
      expect(result.rulesCount).toBe(2);
      expect(result.dnsServerId).toBe('server-1');
      expect(mockUserRulesRepo.setRules).toHaveBeenCalledWith('server-1', expect.any(Array));
      expect(mockUserRulesRepo.enableRules).toHaveBeenCalledWith('server-1');
    });

    it('should append rules when specified', async () => {
      (mockUserRulesRepo.getRules as jest.Mock).mockResolvedValue({
        enabled: true,
        rules: ['||existing.com^'],
      });
      (mockUserRulesRepo.setRules as jest.Mock).mockResolvedValue(undefined);
      (mockUserRulesRepo.enableRules as jest.Mock).mockResolvedValue(undefined);

      const result = await integration.syncRules('server-1', {
        rules: ['||new.com^'],
        append: true,
      });

      expect(result.success).toBe(true);
      expect(result.rulesCount).toBe(2);
      expect(mockUserRulesRepo.setRules).toHaveBeenCalledWith(
        'server-1',
        expect.arrayContaining(['||existing.com^', '||new.com^'])
      );
    });

    it('should handle errors', async () => {
      (mockUserRulesRepo.setRules as jest.Mock).mockRejectedValue(new Error('API error'));

      const result = await integration.syncRules('server-1', {
        rules: ['||example.com^'],
      });

      expect(result.success).toBe(false);
      expect(result.error).toBe('API error');
    });

    it('should require either rulesPath or rules', async () => {
      const result = await integration.syncRules('server-1', {});

      expect(result.success).toBe(false);
      expect(result.error).toContain('Either rulesPath or rules must be provided');
    });
  });

  describe('syncRulesToDefault', () => {
    it('should sync to default server', async () => {
      (mockDnsServerRepo.getDefault as jest.Mock).mockResolvedValue({
        id: 'default-server',
        name: 'Default',
      });
      (mockUserRulesRepo.setRules as jest.Mock).mockResolvedValue(undefined);
      (mockUserRulesRepo.enableRules as jest.Mock).mockResolvedValue(undefined);

      const result = await integration.syncRulesToDefault({
        rules: ['||example.com^'],
      });

      expect(result.success).toBe(true);
      expect(result.dnsServerId).toBe('default-server');
    });

    it('should handle no default server', async () => {
      (mockDnsServerRepo.getDefault as jest.Mock).mockResolvedValue(undefined);

      const result = await integration.syncRulesToDefault({
        rules: ['||example.com^'],
      });

      expect(result.success).toBe(false);
      expect(result.error).toContain('No default DNS server');
    });
  });

  describe('getRulesDiff', () => {
    it('should calculate diff correctly', async () => {
      (mockUserRulesRepo.getRules as jest.Mock).mockResolvedValue({
        enabled: true,
        rules: ['||old.com^', '||both.com^'],
      });

      const diff = await integration.getRulesDiff('server-1', ['||new.com^', '||both.com^']);

      expect(diff.added).toEqual(['||new.com^']);
      expect(diff.removed).toEqual(['||old.com^']);
      expect(diff.unchanged).toEqual(['||both.com^']);
    });
  });
});
