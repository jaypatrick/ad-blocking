/**
 * Tests for CLI argument parsing
 */

import { parseArgs } from './cli';

describe('parseArgs', () => {
  it('should return default options with no arguments', () => {
    const options = parseArgs([]);
    expect(options.copyToRules).toBe(false);
    expect(options.version).toBe(false);
    expect(options.help).toBe(false);
    expect(options.debug).toBe(false);
    expect(options.configPath).toBeUndefined();
    expect(options.outputPath).toBeUndefined();
  });

  it('should parse -c flag for config path', () => {
    const options = parseArgs(['-c', 'config.yaml']);
    expect(options.configPath).toBe('config.yaml');
  });

  it('should parse --config flag for config path', () => {
    const options = parseArgs(['--config', 'config.json']);
    expect(options.configPath).toBe('config.json');
  });

  it('should parse -o flag for output path', () => {
    const options = parseArgs(['-o', 'output.txt']);
    expect(options.outputPath).toBe('output.txt');
  });

  it('should parse --output flag for output path', () => {
    const options = parseArgs(['--output', 'rules.txt']);
    expect(options.outputPath).toBe('rules.txt');
  });

  it('should parse -r flag for copy to rules', () => {
    const options = parseArgs(['-r']);
    expect(options.copyToRules).toBe(true);
  });

  it('should parse --copy-to-rules flag', () => {
    const options = parseArgs(['--copy-to-rules']);
    expect(options.copyToRules).toBe(true);
  });

  it('should parse -f flag for format', () => {
    const options = parseArgs(['-f', 'yaml']);
    expect(options.format).toBe('yaml');
  });

  it('should parse --format flag', () => {
    const options = parseArgs(['--format', 'toml']);
    expect(options.format).toBe('toml');
  });

  it('should throw error for invalid format', () => {
    expect(() => parseArgs(['-f', 'xml'])).toThrow('Invalid format: xml');
  });

  it('should parse -v flag for version', () => {
    const options = parseArgs(['-v']);
    expect(options.version).toBe(true);
  });

  it('should parse --version flag', () => {
    const options = parseArgs(['--version']);
    expect(options.version).toBe(true);
  });

  it('should parse -h flag for help', () => {
    const options = parseArgs(['-h']);
    expect(options.help).toBe(true);
  });

  it('should parse --help flag', () => {
    const options = parseArgs(['--help']);
    expect(options.help).toBe(true);
  });

  it('should parse -d flag for debug', () => {
    const options = parseArgs(['-d']);
    expect(options.debug).toBe(true);
  });

  it('should parse --debug flag', () => {
    const options = parseArgs(['--debug']);
    expect(options.debug).toBe(true);
  });

  it('should parse --show-config flag', () => {
    const options = parseArgs(['--show-config']);
    expect(options.showConfig).toBe(true);
  });

  it('should parse positional config path', () => {
    const options = parseArgs(['config.yaml']);
    expect(options.configPath).toBe('config.yaml');
  });

  it('should parse multiple flags together', () => {
    const options = parseArgs(['-c', 'config.yaml', '-o', 'output.txt', '-r', '-d']);
    expect(options.configPath).toBe('config.yaml');
    expect(options.outputPath).toBe('output.txt');
    expect(options.copyToRules).toBe(true);
    expect(options.debug).toBe(true);
  });

  it('should parse --rules-dir flag', () => {
    const options = parseArgs(['--rules-dir', '/custom/rules']);
    expect(options.rulesDirectory).toBe('/custom/rules');
  });
});
