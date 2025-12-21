/**
 * Tests for configuration validation and input sanitization
 */

import {
  validateConfiguration,
  assertValidConfiguration,
  containsPathTraversal,
  validateUrl,
  validateSourcePath,
  checkFileSize,
  checkSourceCount,
  DEFAULT_RESOURCE_LIMITS,
} from './validation';
import { ConfigurationError, ResourceLimitError } from './errors';

describe('validateConfiguration', () => {
  it('should validate a valid minimal configuration', () => {
    const config = {
      name: 'Test Filter',
      sources: [{ source: 'https://example.com/filter.txt' }],
    };

    const result = validateConfiguration(config);
    expect(result.valid).toBe(true);
    expect(result.errors).toHaveLength(0);
  });

  it('should validate a complete configuration', () => {
    const config = {
      name: 'Test Filter',
      description: 'A test filter',
      version: '1.0.0',
      license: 'MIT',
      homepage: 'https://example.com',
      sources: [
        {
          source: 'https://example.com/filter.txt',
          name: 'Main filter',
          type: 'adblock',
          transformations: ['RemoveComments', 'Deduplicate'],
        },
      ],
      transformations: ['RemoveEmptyLines'],
      inclusions: ['/pattern/'],
      exclusions: ['/excluded/'],
    };

    const result = validateConfiguration(config);
    expect(result.valid).toBe(true);
    expect(result.errors).toHaveLength(0);
  });

  it('should reject non-object configuration', () => {
    expect(validateConfiguration(null).valid).toBe(false);
    expect(validateConfiguration('string').valid).toBe(false);
    expect(validateConfiguration(123).valid).toBe(false);
  });

  it('should require name field', () => {
    const config = { sources: [{ source: 'https://example.com' }] };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('name'))).toBe(true);
  });

  it('should reject empty name', () => {
    const config = { name: '   ', sources: [{ source: 'https://example.com' }] };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('name'))).toBe(true);
  });

  it('should require sources field', () => {
    const config = { name: 'Test' };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('sources'))).toBe(true);
  });

  it('should reject empty sources array', () => {
    const config = { name: 'Test', sources: [] };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('sources'))).toBe(true);
  });

  it('should validate source objects', () => {
    const config = {
      name: 'Test',
      sources: [{ name: 'Missing source field' }],
    };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('sources[0].source'))).toBe(true);
  });

  it('should validate source type values', () => {
    const config = {
      name: 'Test',
      sources: [{ source: 'https://example.com', type: 'invalid' }],
    };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('type'))).toBe(true);
  });

  it('should validate transformation names', () => {
    const config = {
      name: 'Test',
      sources: [{ source: 'https://example.com' }],
      transformations: ['InvalidTransformation'],
    };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(false);
    expect(result.errors.some(e => e.includes('InvalidTransformation'))).toBe(true);
  });

  it('should accept valid transformations', () => {
    const config = {
      name: 'Test',
      sources: [{ source: 'https://example.com' }],
      transformations: ['RemoveComments', 'Compress', 'Deduplicate'],
    };
    const result = validateConfiguration(config);

    expect(result.valid).toBe(true);
  });

  it('should warn about invalid homepage URL', () => {
    const config = {
      name: 'Test',
      sources: [{ source: 'https://example.com' }],
      homepage: 'not-a-url',
    };
    const result = validateConfiguration(config);

    expect(result.warnings.some(w => w.includes('homepage'))).toBe(true);
  });
});

describe('assertValidConfiguration', () => {
  it('should not throw for valid configuration', () => {
    const config = {
      name: 'Test',
      sources: [{ source: 'https://example.com' }],
    };

    expect(() => assertValidConfiguration(config)).not.toThrow();
  });

  it('should throw ConfigurationError for invalid configuration', () => {
    const config = { name: '' };

    expect(() => assertValidConfiguration(config)).toThrow(ConfigurationError);
  });
});

describe('containsPathTraversal', () => {
  it('should detect .. sequences', () => {
    expect(containsPathTraversal('../file.txt')).toBe(true);
    expect(containsPathTraversal('path/../file.txt')).toBe(true);
    expect(containsPathTraversal('../../etc/passwd')).toBe(true);
  });

  it('should detect null bytes', () => {
    expect(containsPathTraversal('file\0.txt')).toBe(true);
  });

  it('should allow safe relative paths', () => {
    expect(containsPathTraversal('file.txt')).toBe(false);
    expect(containsPathTraversal('./file.txt')).toBe(false);
    expect(containsPathTraversal('path/to/file.txt')).toBe(false);
  });

  it('should allow absolute paths', () => {
    expect(containsPathTraversal('/absolute/path/file.txt')).toBe(false);
  });
});

describe('validateUrl', () => {
  it('should accept http URLs', () => {
    expect(validateUrl('http://example.com')).toBe(true);
    expect(validateUrl('http://example.com/path')).toBe(true);
  });

  it('should accept https URLs', () => {
    expect(validateUrl('https://example.com')).toBe(true);
    expect(validateUrl('https://example.com/path?query=1')).toBe(true);
  });

  it('should reject other protocols', () => {
    expect(validateUrl('ftp://example.com')).toBe(false);
    expect(validateUrl('file:///path')).toBe(false);
    expect(validateUrl('javascript:alert(1)')).toBe(false);
  });

  it('should reject invalid URLs', () => {
    expect(validateUrl('not-a-url')).toBe(false);
    expect(validateUrl('')).toBe(false);
  });
});

describe('validateSourcePath', () => {
  it('should validate http URLs', () => {
    const result = validateSourcePath('https://example.com/filter.txt');
    expect(result.valid).toBe(true);
    expect(result.isUrl).toBe(true);
  });

  it('should validate file paths', () => {
    const result = validateSourcePath('filters/list.txt');
    expect(result.valid).toBe(true);
    expect(result.isUrl).toBe(false);
  });

  it('should reject path traversal in file paths', () => {
    const result = validateSourcePath('../../../etc/passwd');
    expect(result.valid).toBe(false);
    expect(result.error).toContain('traversal');
  });

  it('should reject empty source', () => {
    const result = validateSourcePath('');
    expect(result.valid).toBe(false);
  });
});

describe('checkFileSize', () => {
  it('should not throw when under limit', () => {
    expect(() => checkFileSize(1000, 2000, 'test')).not.toThrow();
  });

  it('should throw ResourceLimitError when over limit', () => {
    expect(() => checkFileSize(3000, 2000, 'test')).toThrow(ResourceLimitError);
  });
});

describe('checkSourceCount', () => {
  it('should not throw when under limit', () => {
    expect(() => checkSourceCount(10, 100)).not.toThrow();
  });

  it('should throw ResourceLimitError when over limit', () => {
    expect(() => checkSourceCount(200, 100)).toThrow(ResourceLimitError);
  });

  it('should use default limit', () => {
    expect(() => checkSourceCount(10)).not.toThrow();
    expect(() => checkSourceCount(DEFAULT_RESOURCE_LIMITS.maxSources + 1)).toThrow(ResourceLimitError);
  });
});

describe('DEFAULT_RESOURCE_LIMITS', () => {
  it('should have reasonable defaults', () => {
    expect(DEFAULT_RESOURCE_LIMITS.maxConfigFileSize).toBeGreaterThan(0);
    expect(DEFAULT_RESOURCE_LIMITS.maxOutputFileSize).toBeGreaterThan(0);
    expect(DEFAULT_RESOURCE_LIMITS.maxSources).toBeGreaterThan(0);
    expect(DEFAULT_RESOURCE_LIMITS.compilationTimeoutMs).toBeGreaterThan(0);
    expect(DEFAULT_RESOURCE_LIMITS.maxPathLength).toBeGreaterThan(0);
  });
});
