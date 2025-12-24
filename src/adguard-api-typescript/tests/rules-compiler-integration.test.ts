/**
 * Rules compiler integration tests
 * Deno-native testing implementation
 */

import { assertEquals, assertStringIncludes } from '@std/assert';
import { returnsNext, stub } from '@std/testing/mock';
import { RulesCompilerIntegration } from '../src/rules-compiler-integration.ts';
import type { UserRulesRepository } from '../src/repositories/user-rules.ts';
import type { DnsServerRepository } from '../src/repositories/dns-server.ts';

// Create mock repositories
function createMockUserRulesRepo(): UserRulesRepository {
  return {
    getRules: stub(
      {} as UserRulesRepository,
      'getRules',
      returnsNext([Promise.resolve({ enabled: true, rules: [] })]),
    ),
    setRules: stub(
      {} as UserRulesRepository,
      'setRules',
      returnsNext([Promise.resolve()]),
    ),
    enableRules: stub(
      {} as UserRulesRepository,
      'enableRules',
      returnsNext([Promise.resolve()]),
    ),
    disableRules: stub(
      {} as UserRulesRepository,
      'disableRules',
      returnsNext([Promise.resolve()]),
    ),
  } as unknown as UserRulesRepository;
}

function createMockDnsServerRepo(): DnsServerRepository {
  return {
    getDefault: stub(
      {} as DnsServerRepository,
      'getDefault',
      returnsNext([Promise.resolve({ id: 'default-server', name: 'Default' })]),
    ),
    getAll: stub(
      {} as DnsServerRepository,
      'getAll',
      returnsNext([Promise.resolve([])]),
    ),
  } as unknown as DnsServerRepository;
}

// parseRules tests
Deno.test('RulesCompilerIntegration.parseRules - parses rules from content', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const content = `||example.com^
||ads.example.org^
||tracker.net^`;

  const rules = integration.parseRules(content);
  assertEquals(rules.length, 3);
  assertEquals(rules[0], '||example.com^');
});

Deno.test('RulesCompilerIntegration.parseRules - filters comments when requested', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const content = `! This is a comment
||example.com^
# Another comment
||ads.example.org^`;

  const rules = integration.parseRules(content, true);
  assertEquals(rules.length, 2);
  assertEquals(rules.includes('! This is a comment'), false);
});

Deno.test('RulesCompilerIntegration.parseRules - keeps comments when not filtering', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const content = `! Comment
||example.com^`;

  const rules = integration.parseRules(content, false);
  assertEquals(rules.length, 2);
});

Deno.test('RulesCompilerIntegration.parseRules - filters empty lines', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const content = `||example.com^

||ads.example.org^

`;

  const rules = integration.parseRules(content, true);
  assertEquals(rules.length, 2);
});

// validateRules tests
Deno.test('RulesCompilerIntegration.validateRules - validates correct rules', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['||example.com^', '||ads.example.org^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, true);
  assertEquals(result.errors.length, 0);
});

Deno.test('RulesCompilerIntegration.validateRules - detects rules exceeding max length', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const longDomain = 'a'.repeat(1100);
  const rules = [`||${longDomain}^`];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, false);
  assertEquals(result.errors.length, 1);
  assertStringIncludes(result.errors[0].error, 'exceeds maximum length');
});

Deno.test('RulesCompilerIntegration.validateRules - detects invalid space in domain rules', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['||example.com with space^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, false);
  assertStringIncludes(result.errors[0].error, 'invalid space');
});

Deno.test('RulesCompilerIntegration.validateRules - skips comments in validation', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['! This is a comment', '||example.com^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, true);
});

// syncRules tests
Deno.test('RulesCompilerIntegration.syncRules - syncs inline rules', async () => {
  const mockUserRulesRepo = {
    getRules: () => Promise.resolve({ enabled: true, rules: [] }),
    setRules: () => Promise.resolve(),
    enableRules: () => Promise.resolve(),
    disableRules: () => Promise.resolve(),
  } as unknown as UserRulesRepository;
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const result = await integration.syncRules('server-1', {
    rules: ['||example.com^', '||ads.org^'],
  });

  assertEquals(result.success, true);
  assertEquals(result.rulesCount, 2);
  assertEquals(result.dnsServerId, 'server-1');
});

Deno.test('RulesCompilerIntegration.syncRules - requires either rulesPath or rules', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const result = await integration.syncRules('server-1', {});

  assertEquals(result.success, false);
  assertStringIncludes(result.error || '', 'Either rulesPath or rules must be provided');
});

// syncRulesToDefault tests
Deno.test('RulesCompilerIntegration.syncRulesToDefault - syncs to default server', async () => {
  const mockUserRulesRepo = {
    getRules: () => Promise.resolve({ enabled: true, rules: [] }),
    setRules: () => Promise.resolve(),
    enableRules: () => Promise.resolve(),
    disableRules: () => Promise.resolve(),
  } as unknown as UserRulesRepository;
  const mockDnsServerRepo = {
    getDefault: () => Promise.resolve({ id: 'default-server', name: 'Default' }),
    getAll: () => Promise.resolve([]),
  } as unknown as DnsServerRepository;
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const result = await integration.syncRulesToDefault({
    rules: ['||example.com^'],
  });

  assertEquals(result.success, true);
  assertEquals(result.dnsServerId, 'default-server');
});

Deno.test('RulesCompilerIntegration.syncRulesToDefault - handles no default server', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = {
    getDefault: () => Promise.resolve(undefined),
    getAll: () => Promise.resolve([]),
  } as unknown as DnsServerRepository;
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const result = await integration.syncRulesToDefault({
    rules: ['||example.com^'],
  });

  assertEquals(result.success, false);
  assertStringIncludes(result.error || '', 'No default DNS server');
});

// getRulesDiff tests
Deno.test('RulesCompilerIntegration.getRulesDiff - calculates diff correctly', async () => {
  const mockUserRulesRepo = {
    getRules: () => Promise.resolve({ enabled: true, rules: ['||old.com^', '||both.com^'] }),
    setRules: () => Promise.resolve(),
    enableRules: () => Promise.resolve(),
    disableRules: () => Promise.resolve(),
  } as unknown as UserRulesRepository;
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const diff = await integration.getRulesDiff('server-1', ['||new.com^', '||both.com^']);

  assertEquals(diff.added, ['||new.com^']);
  assertEquals(diff.removed, ['||old.com^']);
  assertEquals(diff.unchanged, ['||both.com^']);
});
