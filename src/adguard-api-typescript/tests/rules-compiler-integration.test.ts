/**
 * Rules compiler integration tests
 */

import { assertEquals, assertArrayIncludes } from '@std/assert';
import { stub, returnsNext, type Stub } from '@std/testing/mock';
import { RulesCompilerIntegration } from '../src/rules-compiler-integration.ts';
import type { UserRulesRepository } from '../src/repositories/user-rules.ts';
import type { DnsServerRepository } from '../src/repositories/dns-server.ts';
import type { DNSServer } from '../src/models/dns-server.ts';
import { BlockingMode } from '../src/models/enums.ts';

// Create a minimal mock DNSServer for testing
function createMockDnsServer(overrides: Partial<DNSServer> = {}): DNSServer {
  return {
    id: 'mock-server',
    name: 'Mock Server',
    default: false,
    device_ids: [],
    settings: {
      protection_enabled: true,
      ip_log_enabled: false,
      auto_connect_devices_enabled: false,
      block_chrome_prefetch: false,
      block_firefox_canary: false,
      block_private_relay: false,
      block_ttl_seconds: 0,
      blocking_mode_settings: {
        blocking_mode: BlockingMode.NXDOMAIN,
      },
      access_settings: {
        enabled: false,
        allowed_clients: [],
        blocked_clients: [],
        blocked_domain_rules: [],
        block_known_scanners: false,
      },
      user_rules_settings: {
        enabled: true,
        rules: [],
      },
      filter_lists_settings: {
        enabled: true,
        filter_list: [],
      },
      safebrowsing_settings: {
        enabled: false,
        block_nrd: false,
      },
      parental_control_settings: {
        enabled: false,
        safe_search_enabled: false,
        youtube_restricted_mode: false,
        block_adult: false,
        blocked_web_services: [],
        schedule: {
          enabled: false,
          timezone: 'UTC',
          schedule_days: [],
        },
      },
    },
    ...overrides,
  };
}

// Create mock repositories with stubs
function createMockUserRulesRepo(): UserRulesRepository & { _stubs: Record<string, Stub<UserRulesRepository>> } {
  const repo = {
    getRules: () => Promise.resolve({ enabled: true, rules: [] as string[] }),
    setRules: () => Promise.resolve(),
    enableRules: () => Promise.resolve(),
    disableRules: () => Promise.resolve(),
    _stubs: {} as Record<string, Stub<UserRulesRepository>>,
  } as unknown as UserRulesRepository & { _stubs: Record<string, Stub<UserRulesRepository>> };
  return repo;
}

function createMockDnsServerRepo(): DnsServerRepository & { _stubs: Record<string, Stub<DnsServerRepository>> } {
  const repo = {
    getDefault: () => Promise.resolve(undefined),
    getAll: () => Promise.resolve([]),
    _stubs: {} as Record<string, Stub<DnsServerRepository>>,
  } as unknown as DnsServerRepository & { _stubs: Record<string, Stub<DnsServerRepository>> };
  return repo;
}

// parseRules tests
Deno.test('RulesCompilerIntegration.parseRules - should parse rules from content', () => {
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

Deno.test('RulesCompilerIntegration.parseRules - should filter comments when requested', () => {
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

Deno.test('RulesCompilerIntegration.parseRules - should keep comments when not filtering', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const content = `! Comment
||example.com^`;

  const rules = integration.parseRules(content, false);
  assertEquals(rules.length, 2);
});

Deno.test('RulesCompilerIntegration.parseRules - should filter empty lines', () => {
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
Deno.test('RulesCompilerIntegration.validateRules - should validate correct rules', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['||example.com^', '||ads.example.org^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, true);
  assertEquals(result.errors.length, 0);
});

Deno.test('RulesCompilerIntegration.validateRules - should detect rules exceeding max length', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const longDomain = 'a'.repeat(1100);
  const rules = [`||${longDomain}^`];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, false);
  assertEquals(result.errors.length, 1);
  assertEquals(result.errors[0].error.includes('exceeds maximum length'), true);
});

Deno.test('RulesCompilerIntegration.validateRules - should detect invalid space in domain rules', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['||example.com with space^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, false);
  assertEquals(result.errors[0].error.includes('invalid space'), true);
});

Deno.test('RulesCompilerIntegration.validateRules - should skip comments in validation', () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const rules = ['! This is a comment', '||example.com^'];
  const result = integration.validateRules(rules);
  assertEquals(result.valid, true);
});

// syncRules tests
Deno.test('RulesCompilerIntegration.syncRules - should sync inline rules', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const setRulesStub = stub(mockUserRulesRepo, 'setRules', returnsNext([Promise.resolve()]));
  const enableRulesStub = stub(mockUserRulesRepo, 'enableRules', returnsNext([Promise.resolve()]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const result = await integration.syncRules('server-1', {
      rules: ['||example.com^', '||ads.org^'],
    });

    assertEquals(result.success, true);
    assertEquals(result.rulesCount, 2);
    assertEquals(result.dnsServerId, 'server-1');
    assertEquals(setRulesStub.calls.length, 1);
    assertEquals(setRulesStub.calls[0].args[0], 'server-1');
    assertEquals(enableRulesStub.calls.length, 1);
  } finally {
    setRulesStub.restore();
    enableRulesStub.restore();
  }
});

Deno.test('RulesCompilerIntegration.syncRules - should append rules when specified', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const getRulesStub = stub(mockUserRulesRepo, 'getRules', returnsNext([
    Promise.resolve({ enabled: true, rules: ['||existing.com^'] }),
  ]));
  const setRulesStub = stub(mockUserRulesRepo, 'setRules', returnsNext([Promise.resolve()]));
  const enableRulesStub = stub(mockUserRulesRepo, 'enableRules', returnsNext([Promise.resolve()]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const result = await integration.syncRules('server-1', {
      rules: ['||new.com^'],
      append: true,
    });

    assertEquals(result.success, true);
    assertEquals(result.rulesCount, 2);
    assertEquals(setRulesStub.calls.length, 1);
    const rulesArg = setRulesStub.calls[0].args[1] as string[];
    assertArrayIncludes(rulesArg, ['||existing.com^', '||new.com^']);
  } finally {
    getRulesStub.restore();
    setRulesStub.restore();
    enableRulesStub.restore();
  }
});

Deno.test('RulesCompilerIntegration.syncRules - should handle errors', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const setRulesStub = stub(mockUserRulesRepo, 'setRules', returnsNext([
    Promise.reject(new Error('API error')),
  ]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const result = await integration.syncRules('server-1', {
      rules: ['||example.com^'],
    });

    assertEquals(result.success, false);
    assertEquals(result.error, 'API error');
  } finally {
    setRulesStub.restore();
  }
});

Deno.test('RulesCompilerIntegration.syncRules - should require either rulesPath or rules', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  const result = await integration.syncRules('server-1', {});

  assertEquals(result.success, false);
  assertEquals(result.error?.includes('Either rulesPath or rules must be provided'), true);
});

// syncRulesToDefault tests
Deno.test('RulesCompilerIntegration.syncRulesToDefault - should sync to default server', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const getDefaultStub = stub(mockDnsServerRepo, 'getDefault', returnsNext([
    Promise.resolve(createMockDnsServer({ id: 'default-server', name: 'Default' })),
  ]));
  const setRulesStub = stub(mockUserRulesRepo, 'setRules', returnsNext([Promise.resolve()]));
  const enableRulesStub = stub(mockUserRulesRepo, 'enableRules', returnsNext([Promise.resolve()]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const result = await integration.syncRulesToDefault({
      rules: ['||example.com^'],
    });

    assertEquals(result.success, true);
    assertEquals(result.dnsServerId, 'default-server');
  } finally {
    getDefaultStub.restore();
    setRulesStub.restore();
    enableRulesStub.restore();
  }
});

Deno.test('RulesCompilerIntegration.syncRulesToDefault - should handle no default server', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const getDefaultStub = stub(mockDnsServerRepo, 'getDefault', returnsNext([
    Promise.resolve(undefined),
  ]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const result = await integration.syncRulesToDefault({
      rules: ['||example.com^'],
    });

    assertEquals(result.success, false);
    assertEquals(result.error?.includes('No default DNS server'), true);
  } finally {
    getDefaultStub.restore();
  }
});

// getRulesDiff tests
Deno.test('RulesCompilerIntegration.getRulesDiff - should calculate diff correctly', async () => {
  const mockUserRulesRepo = createMockUserRulesRepo();
  const mockDnsServerRepo = createMockDnsServerRepo();

  const getRulesStub = stub(mockUserRulesRepo, 'getRules', returnsNext([
    Promise.resolve({ enabled: true, rules: ['||old.com^', '||both.com^'] }),
  ]));

  try {
    const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

    const diff = await integration.getRulesDiff('server-1', ['||new.com^', '||both.com^']);

    assertEquals(diff.added, ['||new.com^']);
    assertEquals(diff.removed, ['||old.com^']);
    assertEquals(diff.unchanged, ['||both.com^']);
  } finally {
    getRulesStub.restore();
  }
});
