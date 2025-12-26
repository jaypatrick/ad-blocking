/**
 * Rules compiler integration tests - Basic validation
 * Full test coverage with mocks to be added in follow-up
 */

import { assertEquals, assert } from '@std/assert';
import { RulesCompilerIntegration } from '../src/rules-compiler-integration.ts';

// Create minimal mock repositories for basic testing
const mockUserRulesRepo = {
  getRules: () => Promise.resolve({ enabled: true, rules: [] }),
  setRules: () => Promise.resolve(undefined),
  enableRules: () => Promise.resolve(undefined),
  disableRules: () => Promise.resolve(undefined),
} as any;

const mockDnsServerRepo = {
  getDefault: () => Promise.resolve({ id: 'test-server', name: 'Test' }),
  getAll: () => Promise.resolve([]),
} as any;

Deno.test('RulesCompilerIntegration', async (t) => {
  const integration = new RulesCompilerIntegration(mockUserRulesRepo, mockDnsServerRepo);

  await t.step('parseRules - should parse rules from content', () => {
    const content = `||example.com^
||ads.example.org^
||tracker.net^`;

    const rules = integration.parseRules(content);
    assertEquals(rules.length, 3);
    assertEquals(rules[0], '||example.com^');
  });

  await t.step('parseRules - should filter comments when requested', () => {
    const content = `! This is a comment
||example.com^
# Another comment
||ads.example.org^`;

    const rules = integration.parseRules(content, true);
    assertEquals(rules.length, 2);
  });

  await t.step('parseRules - should filter empty lines', () => {
    const content = `||example.com^

||ads.example.org^

`;

    const rules = integration.parseRules(content, true);
    assertEquals(rules.length, 2);
  });

  await t.step('validateRules - should validate correct rules', () => {
    const rules = ['||example.com^', '||ads.example.org^'];
    const result = integration.validateRules(rules);
    assertEquals(result.valid, true);
    assertEquals(result.errors.length, 0);
  });

  await t.step('validateRules - should detect rules exceeding max length', () => {
    const longDomain = 'a'.repeat(1100);
    const rules = [`||${longDomain}^`];
    const result = integration.validateRules(rules);
    assertEquals(result.valid, false);
    assertEquals(result.errors.length, 1);
    assert(result.errors[0].error.includes('exceeds maximum length'));
  });

  await t.step('validateRules - should skip comments in validation', () => {
    const rules = ['! This is a comment', '||example.com^'];
    const result = integration.validateRules(rules);
    assertEquals(result.valid, true);
  });
});
