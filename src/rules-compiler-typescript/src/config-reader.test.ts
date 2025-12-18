/**
 * Tests for configuration reader
 */

import { detectFormat } from './config-reader';

describe('detectFormat', () => {
  it('should detect JSON format from .json extension', () => {
    expect(detectFormat('config.json')).toBe('json');
    expect(detectFormat('/path/to/compiler-config.json')).toBe('json');
  });

  it('should detect YAML format from .yaml extension', () => {
    expect(detectFormat('config.yaml')).toBe('yaml');
    expect(detectFormat('/path/to/compiler-config.yaml')).toBe('yaml');
  });

  it('should detect YAML format from .yml extension', () => {
    expect(detectFormat('config.yml')).toBe('yaml');
    expect(detectFormat('/path/to/compiler-config.yml')).toBe('yaml');
  });

  it('should detect TOML format from .toml extension', () => {
    expect(detectFormat('config.toml')).toBe('toml');
    expect(detectFormat('/path/to/compiler-config.toml')).toBe('toml');
  });

  it('should be case-insensitive for extensions', () => {
    expect(detectFormat('config.JSON')).toBe('json');
    expect(detectFormat('config.YAML')).toBe('yaml');
    expect(detectFormat('config.TOML')).toBe('toml');
  });

  it('should throw error for unknown extension', () => {
    expect(() => detectFormat('config.xml')).toThrow('Unknown configuration file extension: .xml');
    expect(() => detectFormat('config.txt')).toThrow('Unknown configuration file extension: .txt');
  });
});
